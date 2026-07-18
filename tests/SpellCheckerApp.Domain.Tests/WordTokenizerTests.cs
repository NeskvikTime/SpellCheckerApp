using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Domain.Tests
{
    public class WordTokenizerTests
    {
        [Fact]
        public void Split_SeparatesWordsFromTheGapsBetweenThem()
        {
            // Arrange
            const string line = "the in";

            // Act
            List<TextSegment> segments = WordTokenizer.Split(line).ToList();

            // Assert
            Assert.Equal(3, segments.Count);
            Assert.True(segments[0].IsWord);
            Assert.Equal("the", segments[0].Value);
            Assert.False(segments[1].IsWord);
            Assert.Equal(" ", segments[1].Value);
            Assert.True(segments[2].IsWord);
            Assert.Equal("in", segments[2].Value);
        }

        [Fact]
        public void Split_LosesNothing_SegmentsRejoinIntoTheOriginalLine()
        {
            // Arrange
            // This is what "print the text lines with whitespace intact" depends on.
            const string line = "  hte   rame\tin  ";

            // Act
            string rejoined = string.Concat(WordTokenizer.Split(line).Select(s => s.Value));

            // Assert
            Assert.Equal(line, rejoined);
        }

        [Fact]
        public void Split_TreatsPunctuationAsAGap()
        {
            // Arrange
            const string line = "the, in";

            // Act
            List<TextSegment> segments = WordTokenizer.Split(line).ToList();

            // Assert
            Assert.Equal(new[] { "the", ", ", "in" }, segments.Select(s => s.Value));
            Assert.Equal(new[] { true, false, true }, segments.Select(s => s.IsWord));
        }

        [Fact]
        public void Split_EmptyLine_ReturnsNoSegments()
        {
            // Arrange
            string line = string.Empty;

            // Act
            List<TextSegment> segments = WordTokenizer.Split(line).ToList();

            // Assert
            Assert.Empty(segments);
        }

        [Fact]
        public void Words_ReturnsOnlyTheLetterRuns()
        {
            // Arrange
            const string line = "rain  spain\tplain";

            // Act
            List<string> words = WordTokenizer.Words(line).ToList();

            // Assert
            Assert.Equal(new[] { "rain", "spain", "plain" }, words);
        }
    }
}