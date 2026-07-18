namespace SpellCheckerApp.Cli;

/// <summary>
/// Where the result of a run goes: next to the input file, with "_out" added to
/// the name. C:\Temp\input.txt becomes C:\Temp\input_out.txt.
/// </summary>
public static class OutputPath
{
    private const string Suffix = "_out";

    public static string For(string inputPath)
    {
        string directory = Path.GetDirectoryName(inputPath) ?? string.Empty;
        string name = Path.GetFileNameWithoutExtension(inputPath);
        string extension = Path.GetExtension(inputPath);

        return Path.Combine(directory, $"{name}{Suffix}{extension}");
    }
}
