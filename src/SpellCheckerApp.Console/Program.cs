using SpellCheckerApp.Cli;

const string folder = @"C:\Temp";
const string defaultFileName = "input.txt";

if (args.Length > 1)
{
    Console.Error.WriteLine("Usage: SpellChecker [file name]");
    Console.Error.WriteLine($"The file is read from {folder}, and defaults to {defaultFileName}.");
    return 2;
}

string requested = args.Length == 1 ? args[0] : defaultFileName;

string inputPath;
try
{
    // Combine looks in C:\Temp for a bare name like "test.txt"
    inputPath = Path.GetFullPath(Path.Combine(folder, requested));

    if (!Path.HasExtension(inputPath) || !string.Equals(Path.GetExtension(inputPath), ".txt", StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine($"'{requested}' is not a usable file name: file must have a .txt extension.");
        return 1;
    }
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"'{requested}' is not a usable file name: {ex.Message}");
    return 1;
}

string outputPath = OutputPath.For(inputPath);

try
{
    // Does nothing if the folder is already there.
    Directory.CreateDirectory(Path.GetDirectoryName(inputPath) ?? folder);

    if (!File.Exists(inputPath))
    {
        // The empty using block matters: File.Create leaves the handle open otherwise.
        using (File.Create(inputPath))
        {
        }

        Console.Error.WriteLine($"Created an empty file: {inputPath}.");
        Console.Error.WriteLine("Fill it in - dictionary, a === line, text lines, another === line - then run again.");
        return 0;
    }

    new FileSpellCheckRunner().Check(inputPath, outputPath);

    Console.Out.WriteLine($"Appended the results to {outputPath}");
    return 0;
}
catch (InvalidDataException ex)
{
    // The input broke a rule from the specification (e.g. a word over 50 characters).
    Console.Error.WriteLine($"The input file is not valid: {ex.Message}");
    return 1;
}
catch (IOException ex)
{
    // Covers a locked file, a full disk, or C:\Temp existing as a file.
    Console.Error.WriteLine($"Could not read or write a file: {ex.Message}");
    return 1;
}
catch (UnauthorizedAccessException ex)
{
    Console.Error.WriteLine($"Access denied: {ex.Message}");
    return 1;
}