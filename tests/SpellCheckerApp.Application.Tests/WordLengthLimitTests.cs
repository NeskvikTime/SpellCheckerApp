namespace SpellCheckerApp.Application.Tests
{
    public class WordLengthLimitTests
    {
        [Fact]
        public void Validate_WordAtTheLimit_DoesNotThrow()
        {
            // Arrange
            string word = new string('a', WordLengthLimit.MaxWordLength - 1);

            // Act
            Exception? exception = Record.Exception(() => WordLengthLimit.Validate(word));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WordOneOverTheLimit_Throws()
        {
            // Arrange
            string word = new string('a', WordLengthLimit.MaxWordLength);

            // Act
            Action act = () => WordLengthLimit.Validate(word);

            // Assert
            Assert.Throws<InvalidDataException>(act);
        }

        [Fact]
        public void Validate_WordWellUnderTheLimit_DoesNotThrow()
        {
            // Arrange
            const string word = "pain";

            // Act
            Exception? exception = Record.Exception(() => WordLengthLimit.Validate(word));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_ExceptionMessage_ReportsHowLongTheWordWas()
        {
            // Arrange
            string word = new string('a', WordLengthLimit.MaxWordLength);

            // Act
            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => WordLengthLimit.Validate(word));

            // Assert
            Assert.Contains(word.Length.ToString(), exception.Message);
        }
    }
}
