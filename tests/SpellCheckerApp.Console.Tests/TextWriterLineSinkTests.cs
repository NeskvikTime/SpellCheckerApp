namespace SpellCheckerApp.Cli.Tests
{
    public class TextWriterLineSinkTests
    {
        [Fact]
        public void Write_PassesTheTextStraightThrough()
        {
            // Arrange
            StringWriter writer = new StringWriter();
            TextWriterLineAdapter sut = new TextWriterLineAdapter(writer);

            // Act
            sut.Write("the ");
            sut.Write("plain");

            // Assert
            Assert.Equal("the plain", writer.ToString());
        }

        [Fact]
        public void WriteLine_WritesALineFeedRatherThanTheEnvironmentNewLine()
        {
            // Arrange
            // Output must not change between Windows and Linux, so this is "\n"
            // deliberately and not Environment.NewLine.
            StringWriter writer = new StringWriter();
            TextWriterLineAdapter sut = new TextWriterLineAdapter(writer);

            // Act
            sut.Write("the");
            sut.WriteLine();

            // Assert
            Assert.Equal("the\n", writer.ToString());
        }

        [Fact]
        public void WriteLine_OnItsOwn_ProducesABlankLine()
        {
            // Arrange
            StringWriter writer = new StringWriter();
            TextWriterLineAdapter sut = new TextWriterLineAdapter(writer);

            // Act
            sut.WriteLine();

            // Assert
            Assert.Equal("\n", writer.ToString());
        }

        [Fact]
        public void Constructor_NullWriter_Throws()
        {
            // Arrange
            TextWriter writer = null;

            // Act
            Action act = () => new TextWriterLineAdapter(writer);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}