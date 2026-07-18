using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Domain.Tests
{
    public class SpellCheckerTests
    {
        // The dictionary from the specification, in its original order.
        private static readonly string[] SampleDictionary =
        {
            "rain", "spain", "plain", "plaint", "pain", "main", "mainly",
            "the", "in", "on", "fall", "falls", "his", "was"
        };

        private static SpellChecker CreateSut()
        {
            return new SpellChecker(new WordDictionary(SampleDictionary));
        }

        [Fact]
        public void Check_WordIsInTheDictionary_ReturnsKnown()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("pain");

            // Assert
            Assert.Equal(CorrectionKind.Known, result.Kind);
        }

        [Fact]
        public void Check_WordIsInTheDictionaryInAnotherCase_StillReturnsKnown()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("PAIN");

            // Assert
            Assert.Equal(CorrectionKind.Known, result.Kind);
        }

        [Fact]
        public void Check_NothingIsCloseEnough_ReturnsUnknown()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("rame");

            // Assert
            Assert.Equal(CorrectionKind.Unknown, result.Kind);
        }

        [Fact]
        public void Check_OnlyRouteNeedsTwoAdjacentDeletions_ReturnsUnknown()
        {
            // Arrange
            // "hints" -> "his" removes the neighbouring 'n' and 't'.
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("hints");

            // Assert
            Assert.Equal(CorrectionKind.Unknown, result.Kind);
        }

        [Fact]
        public void Check_ExactlyOneCorrection_ReturnsIt()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("fells");

            // Assert
            Assert.Equal(CorrectionKind.Corrected, result.Kind);
            Assert.Equal(new[] { "falls" }, result.Suggestions);
        }

        [Fact]
        public void Check_SeveralCorrectionsAtTheSameDistance_ReturnsAllOfThem()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("mainy");

            // Assert
            Assert.Equal(CorrectionKind.Corrected, result.Kind);
            Assert.Equal(new[] { "main", "mainly" }, result.Suggestions);
        }

        [Fact]
        public void Check_SeveralCorrections_AreOrderedByPositionInTheDictionaryNotAlphabetically()
        {
            // Arrange
            // "mainly" is listed before "main" here, so dictionary order and
            // alphabetical order disagree. The dictionary must win.
            SpellChecker sut = new SpellChecker(new WordDictionary(new[] { "mainly", "main" }));

            // Act
            CorrectionResult result = sut.Check("mainy");

            // Assert
            Assert.Equal(new[] { "mainly", "main" }, result.Suggestions);
        }

        [Fact]
        public void Check_OneEditCorrectionExists_DiscardsEveryTwoEditCorrection()
        {
            // Arrange
            // "lain" is one edit from "plain" and two from "rain", "pain", "main"
            // and "plaint". Only "plain" may survive.
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("lain");

            // Assert
            Assert.Equal(new[] { "plain" }, result.Suggestions);
        }

        [Fact]
        public void Check_NoOneEditCorrection_FallsBackToTwoEditCorrections()
        {
            // Arrange
            SpellChecker sut = CreateSut();

            // Act
            CorrectionResult result = sut.Check("teh");

            // Assert
            Assert.Equal(new[] { "the" }, result.Suggestions);
        }

        [Fact]
        public void Check_Suggestions_UseTheCasingFromTheDictionary()
        {
            // Arrange
            SpellChecker sut = new SpellChecker(new WordDictionary(new[] { "Main", "MAINLY" }));

            // Act
            CorrectionResult result = sut.Check("MaInY");

            // Assert
            Assert.Equal(new[] { "Main", "MAINLY" }, result.Suggestions);
        }

        [Fact]
        public void Check_EmptyDictionary_ReturnsUnknown()
        {
            // Arrange
            SpellChecker sut = new SpellChecker(new WordDictionary(new string[0]));

            // Act
            CorrectionResult result = sut.Check("anything");

            // Assert
            Assert.Equal(CorrectionKind.Unknown, result.Kind);
        }

        [Fact]
        public void Constructor_NullDictionary_Throws()
        {
            // Arrange
            WordDictionary dictionary = null;

            // Act
            Action act = () => new SpellChecker(dictionary);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}