namespace SpellCheckerApp.Cli.Tests
{
    /// <summary>
    /// No substitutes here on purpose: StringReader is already a perfectly good
    /// in-memory TextReader, and a real one proves more than a mock would.
    /// </summary>
    public class TextReaderLineSourceTests
    {
        [Fact]
        public void ReadLine_ReturnsEachLineInOrder()
        {
            // Arrange
            StringReader reader = new StringReader("first\nsecond");
            TextReaderLineAdapter sut = new TextReaderLineAdapter(reader);

            // Act
            string first = sut.ReadLine();
            string second = sut.ReadLine();

            // Assert
            Assert.Equal("first", first);
            Assert.Equal("second", second);
        }

        [Fact]
        public void ReadLine_WhenThereIsNothingLeft_ReturnsNull()
        {
            // Arrange
            StringReader reader = new StringReader("only");
            TextReaderLineAdapter sut = new TextReaderLineAdapter(reader);
            sut.ReadLine();

            // Act
            string next = sut.ReadLine();

            // Assert
            Assert.Null(next);
        }

        [Fact]
        public void ReadLine_EmptyInput_ReturnsNullImmediately()
        {
            // Arrange
            StringReader reader = new StringReader(string.Empty);
            TextReaderLineAdapter sut = new TextReaderLineAdapter(reader);

            // Act
            string line = sut.ReadLine();

            // Assert
            Assert.Null(line);
        }

        [Fact]
        public void ReadLine_BlankLine_ReturnsAnEmptyStringNotNull()
        {
            // Arrange
            StringReader reader = new StringReader("\nafter");
            TextReaderLineAdapter sut = new TextReaderLineAdapter(reader);

            // Act
            string line = sut.ReadLine();

            // Assert
            Assert.Equal(string.Empty, line);
        }

        [Fact]
        public void Constructor_NullReader_Throws()
        {
            // Arrange
            TextReader reader = null;

            // Act
            Action act = () => new TextReaderLineAdapter(reader);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}