namespace UnitTests;

/*
Writes throwaway data files for the tests. Every call uses a unique
file name because the tests run in parallel.
*/
internal static class DataFileTestHelper
{
    // Creates a temp data file holding lines and returns its path.
    public static string WriteFile(params string[] lines)
    {
        string path = NewPath();
        File.WriteAllLines(path, lines);
        return path;
    }

    // Returns a path in the temp folder where no file exists.
    public static string NewPath() =>
        Path.Combine(Path.GetTempPath(), $"scadahmi-{Guid.NewGuid():N}.csv");

    // Builds count valid lines whose values are 1, 2, 3, ...
    public static string[] ValidLines(int count) =>
        Enumerable.Range(1, count)
            .Select(i => $"2026-11-16T19:00:{i:00},{i}.5,%,Ok")
            .ToArray();
}
