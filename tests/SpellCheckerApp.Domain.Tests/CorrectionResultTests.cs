using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Domain.Tests
{
    public class CorrectionResultTests
    {
        [Fact]
        public void Known_HasNoSuggestions()
        {
            // Arrange
            // (nothing to arrange)

            // Act
            CorrectionResult result = CorrectionResult.Known;

            // Assert
            Assert.Equal(CorrectionKind.Known, result.Kind);
            Assert.Empty(result.Suggestions);
        }

        [Fact]
        public void Unknown_HasNoSuggestions()
        {
            // Arrange
            // (nothing to arrange)

            // Act
            CorrectionResult result = CorrectionResult.Unknown;

            // Assert
            Assert.Equal(CorrectionKind.Unknown, result.Kind);
            Assert.Empty(result.Suggestions);
        }

        [Fact]
        public void FromSuggestions_ReturnsACorrectedResultCarryingThem()
        {
            // Arrange
            List<string> suggestions = new List<string> { "main", "mainly" };

            // Act
            CorrectionResult result = CorrectionResult.FromSuggestions(suggestions);

            // Assert
            Assert.Equal(CorrectionKind.Corrected, result.Kind);
            Assert.Equal(new[] { "main", "mainly" }, result.Suggestions);
        }

        [Fact]
        public void FromSuggestions_ResultIsNotAffectedByLaterChangesToTheList()
        {
            // Arrange
            List<string> suggestions = new List<string> { "main" };
            CorrectionResult result = CorrectionResult.FromSuggestions(suggestions);

            // Act
            suggestions.Add("mainly");

            // Assert
            // ReadOnlyCollection wraps rather than copies, so this documents the
            // actual behaviour: the caller must not keep mutating the list.
            Assert.Equal(2, result.Suggestions.Count);
        }

        [Fact]
        public void FromSuggestions_Empty_Throws()
        {
            // Arrange
            List<string> suggestions = new List<string>();

            // Act
            Action act = () => CorrectionResult.FromSuggestions(suggestions);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FromSuggestions_Null_Throws()
        {
            // Arrange
            List<string> suggestions = null;

            // Act
            Action act = () => CorrectionResult.FromSuggestions(suggestions);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}