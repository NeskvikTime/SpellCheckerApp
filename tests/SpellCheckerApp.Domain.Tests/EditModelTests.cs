using SpellCheckerApp.Domain.Editing;

namespace SpellCheckerApp.Domain.Tests
{
    public class EditModelTests
    {
        [Theory]
        // identical
        [InlineData("main", "main", 0)]
        // one deletion from the text word
        [InlineData("mainy", "main", 1)]
        [InlineData("oon", "on", 1)]
        // one insertion into the text word
        [InlineData("mainy", "mainly", 1)]
        [InlineData("lain", "plain", 1)]
        // substitution == one delete plus one insert
        [InlineData("fells", "falls", 2)]
        // moved letter == one delete plus one insert
        [InlineData("hte", "the", 2)]
        [InlineData("teh", "the", 2)]
        [InlineData("pliant", "plaint", 2)]
        // two non-adjacent insertions
        [InlineData("lain", "plaint", 2)]
        public void Distance_WhenReachable_ReturnsNumberOfEdits(string text, string candidate, int expected)
        {
            // Arrange
            // (inputs supplied by the theory)

            // Act
            int? distance = EditModel.Distance(text, candidate);

            // Assert
            Assert.Equal(expected, distance);
        }

        [Theory]
        // nothing in common
        [InlineData("rame", "rain")]
        [InlineData("rame", "main")]
        // length difference of three or more
        [InlineData("mainly", "in")]
        [InlineData("in", "plain")]
        public void Distance_WhenUnreachable_ReturnsNull(string text, string candidate)
        {
            // Arrange
            // (inputs supplied by the theory)

            // Act
            int? distance = EditModel.Distance(text, candidate);

            // Assert
            Assert.Null(distance);
        }

        [Fact]
        public void Distance_TwoDeletionsOfAdjacentLetters_ReturnsNull()
        {
            // Arrange
            // "hints" can only reach "his" by deleting the 'n' and the 't', which sit
            // next to each other. The specification forbids that, so there is no path.

            // Act
            int? distance = EditModel.Distance("hints", "his");

            // Assert
            Assert.Null(distance);
        }

        [Fact]
        public void Distance_TwoInsertionsOfAdjacentLetters_ReturnsNull()
        {
            // Arrange
            // The mirror image of the case above: inserting 'n' and 't' into "his"
            // would place them side by side.

            // Act
            int? distance = EditModel.Distance("his", "hints");

            // Assert
            Assert.Null(distance);
        }

        [Fact]
        public void Distance_TwoDeletionsOfSeparatedLetters_ReturnsTwo()
        {
            // Arrange
            // "plaint" reaches "lain" by dropping the 'p' at the front and the 't' at
            // the back. Same kind of edit, but not adjacent, so it is allowed.

            // Act
            int? distance = EditModel.Distance("plaint", "lain");

            // Assert
            Assert.Equal(2, distance);
        }

        [Fact]
        public void Distance_MixedInsertAndDelete_IgnoresTheAdjacencyRestriction()
        {
            // Arrange
            // "hte" -> "the" deletes the 'h' and re-inserts it one position later.
            // The two positions touch, but the restriction only applies when both
            // edits are the same kind.

            // Act
            int? distance = EditModel.Distance("hte", "the");

            // Assert
            Assert.Equal(2, distance);
        }
    }
}