using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Application.Tests
{
    public class CorrectionResultFormatterTests
    {
        [Fact]
        public void Format_KnownWord_PrintsItWithTheCasingFromTheText()
        {
            // Arrange
            CorrectionResult result = CorrectionResult.Known;

            // Act
            string formatted = CorrectionResultFormatter.Format("Pain", result);

            // Assert
            Assert.Equal("Pain", formatted);
        }

        [Fact]
        public void Format_UnknownWord_WrapsTheOriginalInBracesWithAQuestionMark()
        {
            // Arrange
            CorrectionResult result = CorrectionResult.Unknown;

            // Act
            string formatted = CorrectionResultFormatter.Format("Rame", result);

            // Assert
            Assert.Equal("{Rame?}", formatted);
        }

        [Fact]
        public void Format_ASingleCorrection_PrintsItBare()
        {
            // Arrange
            CorrectionResult result = CorrectionResult.FromSuggestions(new List<string> { "falls" });

            // Act
            string formatted = CorrectionResultFormatter.Format("fells", result);

            // Assert
            Assert.Equal("falls", formatted);
        }

        [Fact]
        public void Format_SeveralCorrections_WrapsThemInBracesSeparatedBySpaces()
        {
            // Arrange
            CorrectionResult result = CorrectionResult.FromSuggestions(new List<string> { "main", "mainly" });

            // Act
            string formatted = CorrectionResultFormatter.Format("mainy", result);

            // Assert
            Assert.Equal("{main mainly}", formatted);
        }

        [Fact]
        public void Format_ThreeCorrections_KeepsTheOrderItWasGiven()
        {
            // Arrange
            CorrectionResult result = CorrectionResult.FromSuggestions(new List<string> { "rain", "pain", "main" });

            // Act
            string formatted = CorrectionResultFormatter.Format("xain", result);

            // Assert
            Assert.Equal("{rain pain main}", formatted);
        }
    }
}