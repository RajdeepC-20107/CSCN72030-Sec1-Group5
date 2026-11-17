using System.Globalization;
using System.Text;

namespace SCADAHMI_Project;

/* Reads one device's simulated data file and hands back one sample per
call, looping to the first line after the last one (REQ-SYS-010).

File format: ASCII text, one sample per line, four comma-separated fields

Timestamp,Value,Unit,Status
2026-11-16T19:00:00,35.0,%,Ok

Timestamp  ISO 8601 date and time (yyyy-MM-ddTHH:mm:ss)
Value      number, '.' as the decimal separator
Unit       free text without commas (may be empty)
Status     Ok | Fault | NoPower | Disconnected

Blank lines and lines starting with '#' are ignored, so a file can carry
comments. Any other line that does not parse makes the whole file corrupt.
*/
public sealed class DataFileReader
{
    private const int FieldCount = 4;

    private readonly string filePath;
    private List<Sample>? samples;
    private int nextIndex;

    // filePath: Path of the data file. It is not opened until the first read.
    public DataFileReader(string filePath)
    {
        this.filePath = filePath;
    }

    // Path of the data file this reader loops over.
    public string FilePath => filePath;

    /*
     * Returns the next sample in file order as a DeviceReading
     * for deviceId, wrapping to line 1 after the last line.
     *
     * Throws DataFileException:
     * The file is missing, unreadable, empty or corrupt. The file is loaded
     * again on the next call, so the reader recovers once the file is fixed.
     */
    public DeviceReading ReadNext(string deviceId)
    {
        samples ??= Load(filePath);

        Sample sample = samples[nextIndex];
        nextIndex = (nextIndex + 1) % samples.Count;

        return new DeviceReading(
            deviceId, sample.Timestamp, sample.Value, sample.Unit, sample.Status
        );
    }

    /*
     * Forgets the loaded samples and the position, so the next read loads
     * the file again and starts from line 1.
     */
    public void Reset()
    {
        samples = null;
        nextIndex = 0;
    }

    private static List<Sample> Load(string path)
    {
        string[] lines;

        try
        {
            lines = File.ReadAllLines(path, Encoding.ASCII);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            // FileNotFoundException and DirectoryNotFoundException are IOExceptions.
            throw new DataFileException($"Cannot read data file '{path}': {e.Message}", e);
        }

        List<Sample> loaded = [];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            loaded.Add(ParseLine(line, path, lineNumber: i + 1));
        }

        if (loaded.Count == 0)
        {
            throw new DataFileException($"Data file '{path}' contains no samples.");
        }

        return loaded;
    }

    private static Sample ParseLine(string line, string path, int lineNumber)
    {
        string[] fields = line.Split(',');

        if (fields.Length != FieldCount)
        {
            throw Corrupt(path, lineNumber, $"expected {FieldCount} fields but found {fields.Length}");
        }

        if (!DateTime.TryParse(
                fields[0].Trim(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime timestamp))
        {
            throw Corrupt(path, lineNumber, $"'{fields[0]}' is not a timestamp");
        }

        if (!double.TryParse(
                fields[1].Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value)
            || !double.IsFinite(value))
        {
            throw Corrupt(path, lineNumber, $"'{fields[1]}' is not a number");
        }

        string statusText = fields[3].Trim();

        // Enum.TryParse also accepts numbers ("7"), so check the name is defined.
        if (!Enum.TryParse(statusText, ignoreCase: true, out DeviceStatus status)
            || !Enum.IsDefined(status)
            || char.IsAsciiDigit(statusText[0]))
        {
            throw Corrupt(path, lineNumber, $"'{fields[3]}' is not a device status");
        }

        return new Sample(timestamp, value, fields[2].Trim(), status);
    }

    private static DataFileException Corrupt(string path, int lineNumber, string reason) =>
        new($"Data file '{path}' is corrupt at line {lineNumber}: {reason}.");

    // One parsed line of the data file.
    private readonly record struct Sample(
        DateTime Timestamp, double Value, string Unit, DeviceStatus Status
    );
}