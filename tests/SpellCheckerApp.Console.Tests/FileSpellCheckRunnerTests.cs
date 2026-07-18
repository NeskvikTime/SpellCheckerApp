using System.Text;

namespace SpellCheckerApp.Cli.Tests;

/// <summary>
/// Real files in a real temp folder. Substituting the file system here would only
/// prove that the substitute does what it was told; these prove the encoding, the
/// line endings and the append flag are actually right.
/// </summary>
public class FileSpellCheckRunnerTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    private const string SampleInput =
        "rain spain plain plaint pain main mainly\n" +
        "the in on fall falls his was\n" +
        "===\n" +
        "hte rame in pain fells\n" +
        "mainy oon teh lain\n" +
        "was hints pliant\n" +
        "===\n";

    private const string SampleOutput =
        "the {rame?} in pain falls\n" +
        "{main mainly} on the plain\n" +
        "was {hints?} plaint\n";

    public FileSpellCheckRunnerTests()
    {
        Directory.CreateDirectory(_folder);
    }

    public void Dispose()
    {
        Directory.Delete(_folder, recursive: true);
    }

    private string WriteInput(string content)
    {
        string path = Path.Combine(_folder, "input.txt");
        File.WriteAllText(path, content, new UTF8Encoding(false));
        return path;
    }

    [Fact]
    public void Check_TheExampleFromTheSpecification_WritesTheExpectedFile()
    {
        // Arrange
        string inputPath = WriteInput(SampleInput);
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.Equal(SampleOutput, File.ReadAllText(outputPath));
    }

    [Fact]
    public void Check_OutputFileDoesNotExist_CreatesIt()
    {
        // Arrange
        string inputPath = WriteInput(SampleInput);
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.True(File.Exists(outputPath));
    }

    [Fact]
    public void Check_OutputFileAlreadyExists_AddsToTheEndRatherThanReplacing()
    {
        // Arrange
        string inputPath = WriteInput(SampleInput);
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.Equal(SampleOutput + SampleOutput, File.ReadAllText(outputPath));
    }

    [Fact]
    public void Check_OutputFileHadContentAlready_KeepsIt()
    {
        // Arrange
        string inputPath = WriteInput(SampleInput);
        string outputPath = OutputPath.For(inputPath);
        File.WriteAllText(outputPath, "previous run\n", new UTF8Encoding(false));
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.Equal("previous run\n" + SampleOutput, File.ReadAllText(outputPath));
    }

    [Fact]
    public void Check_NeverWritesAByteOrderMark()
    {
        // Arrange
        // A BOM would be tolerable at the start of the file but not in the middle,
        // and append mode puts it in the middle on the second run.
        string inputPath = WriteInput(SampleInput);
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);
        sut.Check(inputPath, outputPath);

        // Assert
        byte[] bytes = File.ReadAllBytes(outputPath);
        Assert.DoesNotContain((byte)0xEF, bytes);
    }

    [Fact]
    public void Check_InputWrittenWithABom_StillReadsCorrectly()
    {
        // Arrange
        // detectEncodingFromByteOrderMarks: true is what makes this pass. Someone
        // saving the input from Notepad gets a BOM whether they wanted one or not.
        string inputPath = Path.Combine(_folder, "input.txt");
        File.WriteAllText(inputPath, SampleInput, new UTF8Encoding(true));
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.Equal(SampleOutput, File.ReadAllText(outputPath));
    }

    [Fact]
    public void Check_EmptyInputFile_ProducesAnEmptyOutputFile()
    {
        // Arrange
        // This is the state Program leaves things in when it creates a missing input.
        string inputPath = WriteInput(string.Empty);
        string outputPath = OutputPath.For(inputPath);
        var sut = new FileSpellCheckRunner();

        // Act
        sut.Check(inputPath, outputPath);

        // Assert
        Assert.Equal(string.Empty, File.ReadAllText(outputPath));
    }
}
