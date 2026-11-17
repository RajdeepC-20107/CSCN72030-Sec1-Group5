namespace SCADAHMI_Project;

/*Thrown by DataFileReader when a data file is missing,
unreadable, empty or contains a line that does not match the format.
*/
public sealed class DataFileException : Exception
{
    public DataFileException(string message, Exception? inner = null)
        : base(message, inner) { }
}