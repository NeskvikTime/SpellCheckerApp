namespace SpellCheckerApp.Cli.Tests;

public class OutputPathTests
{
    [Theory]
    [InlineData(@"C:\Temp\input.txt", @"C:\Temp\input_out.txt")]
    [InlineData(@"C:\Temp\words.dat", @"C:\Temp\words_out.dat")]
    // GetFileNameWithoutExtension only strips the last extension
    [InlineData(@"C:\Temp\my.data.txt", @"C:\Temp\my.data_out.txt")]
    // no extension at all
    [InlineData(@"C:\Temp\input", @"C:\Temp\input_out")]
    public void For_InsertsOutBeforeTheExtension(string input, string expected)
    {
        // Arrange
        // (inputs supplied by the theory)

        // Act
        string output = OutputPath.For(input);

        // Assert
        Assert.Equal(expected, output);
    }

    [Fact]
    public void For_KeepsTheResultInTheSameFolderAsTheInput()
    {
        // Arrange
        const string input = @"C:\Temp\nested\input.txt";

        // Act
        string output = OutputPath.For(input);

        // Assert
        Assert.Equal(Path.GetDirectoryName(input), Path.GetDirectoryName(output));
    }

    [Fact]
    public void For_BareFileNameWithNoFolder_StillWorks()
    {
        // Arrange
        const string input = "input.txt";

        // Act
        string output = OutputPath.For(input);

        // Assert
        Assert.Equal("input_out.txt", output);
    }
}

