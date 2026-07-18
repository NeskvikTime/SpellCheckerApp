using NSubstitute;
using SpellCheckerApp.Application.Contracts;

namespace SpellCheckerApp.Application.Tests
{
    public class DictionaryReaderTests
    {
        [Fact]
        public void Read_CollectsEveryWordUntilTheSeparator()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("rain spain", "plain plaint", "===", "hte rame", null);

            // Act
            List<string> words = DictionaryReader.Read(source);

            // Assert
            Assert.Equal(new[] { "rain", "spain", "plain", "plaint" }, words);
        }

        [Fact]
        public void Read_LeavesTheTextLinesForTheCaller()
        {
            // Arrange
            // The separator must be consumed, and nothing beyond it.
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("rain", "===", "hte rame", null);

            // Act
            DictionaryReader.Read(source);

            // Assert
            Assert.Equal("hte rame", source.ReadLine());
        }

        [Fact]
        public void Read_SeparatorSurroundedByWhitespace_IsStillRecognised()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("rain", "  ===  ", "hte", null);

            // Act
            List<string> words = DictionaryReader.Read(source);

            // Assert
            Assert.Equal(new[] { "rain" }, words);
        }

        [Fact]
        public void Read_InputEndsWithoutASeparator_ReturnsWhatWasFound()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("rain spain", (string?)null);

            // Act
            List<string> words = DictionaryReader.Read(source);

            // Assert
            Assert.Equal(new[] { "rain", "spain" }, words);
        }

        [Fact]
        public void Read_EmptyDictionarySection_ReturnsNoWords()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("===", null!);

            // Act
            List<string> words = DictionaryReader.Read(source);

            // Assert
            Assert.Empty(words);
        }

        [Fact]
        public void Read_DoesNotDeduplicate_ThatIsTheDictionarysJob()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the The", "===", null);

            // Act
            List<string> words = DictionaryReader.Read(source);

            // Assert
            Assert.Equal(new[] { "the", "The" }, words);
        }

        [Fact]
        public void Read_DictionaryWordOverTheLimit_Throws()
        {
            // Arrange
            // The spec's "up to 50 characters" rule applies to dictionary words too.
            string tooLong = new string('a', WordLengthLimit.MaxWordLength);
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns(tooLong, "===", null);

            // Act
            Action act = () => DictionaryReader.Read(source);

            // Assert
            Assert.Throws<InvalidDataException>(act);
        }
    }
}