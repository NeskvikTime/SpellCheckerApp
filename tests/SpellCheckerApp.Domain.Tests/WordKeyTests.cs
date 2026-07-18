using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Domain.Tests
{
    public class WordKeyTests
    {
        [Theory]
        [InlineData("The", "the")]
        [InlineData("MAINLY", "mainly")]
        [InlineData("pain", "pain")]
        public void For_ReturnsTheLowerCasedWord(string word, string expected)
        {
            // Arrange
            // (inputs supplied by the theory)

            // Act
            string key = WordKey.For(word);

            // Assert
            Assert.Equal(expected, key);
        }
    }
}