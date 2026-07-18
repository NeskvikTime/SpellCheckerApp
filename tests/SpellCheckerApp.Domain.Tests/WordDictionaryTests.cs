using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Domain.Tests
{
    public class WordDictionaryTests
    {
        [Fact]
        public void Contains_WordFromTheDictionary_ReturnsTrue()
        {
            // Arrange
            WordDictionary dictionary = new WordDictionary(new[] { "rain", "spain" });

            // Act
            bool contains = dictionary.Contains("rain");

            // Assert
            Assert.True(contains);
        }

        [Fact]
        public void Contains_WordNotInTheDictionary_ReturnsFalse()
        {
            // Arrange
            WordDictionary dictionary = new WordDictionary(new[] { "rain", "spain" });

            // Act
            bool contains = dictionary.Contains("plain");

            // Assert
            Assert.False(contains);
        }

        [Fact]
        public void GetWord_ReturnsTheCasingUsedInTheDictionary()
        {
            // Arrange
            WordDictionary dictionary = new WordDictionary(new[] { "The", "MAINLY" });

            // Act
            string first = dictionary.GetWord(0);
            string second = dictionary.GetWord(1);

            // Assert
            Assert.Equal("The", first);
            Assert.Equal("MAINLY", second);
        }

        [Fact]
        public void GetKey_ReturnsTheNormalisedForm()
        {
            // Arrange
            WordDictionary dictionary = new WordDictionary(new[] { "The" });

            // Act
            string key = dictionary.GetKey(0);

            // Assert
            Assert.Equal("the", key);
        }

        [Fact]
        public void Constructor_WordRepeatedInADifferentCase_KeepsOnlyTheFirst()
        {
            // Arrange
            IEnumerable<string> words = new[] { "The", "the", "THE" };

            // Act
            WordDictionary dictionary = new WordDictionary(words);

            // Assert
            Assert.Equal(1, dictionary.Count);
            Assert.Equal("The", dictionary.GetWord(0));
        }

        [Fact]
        public void Constructor_PreservesTheOrderWordsWereRead()
        {
            // Arrange
            // Ids are handed out in reading order; corrections are printed in that order.
            IEnumerable<string> words = new[] { "main", "mainly", "the" };

            // Act
            WordDictionary dictionary = new WordDictionary(words);

            // Assert
            Assert.Equal("main", dictionary.GetWord(0));
            Assert.Equal("mainly", dictionary.GetWord(1));
            Assert.Equal("the", dictionary.GetWord(2));
        }

        [Fact]
        public void Constructor_NullWords_Throws()
        {
            // Arrange
            IEnumerable<string> words = null;

            // Act
            Action act = () => new WordDictionary(words);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void FindCandidates_ReturnsEveryWordThatSharesADeletionVariant()
        {
            // Arrange
            // "mainy" reaches "main" by deleting the 'y', and "mainly" by inserting
            // an 'l'. Both must come back as candidates.
            WordDictionary dictionary = new WordDictionary(new[] { "main", "mainly" });

            // Act
            List<int> candidates = dictionary.FindCandidates("mainy").ToList();

            // Assert
            Assert.Contains(0, candidates);
            Assert.Contains(1, candidates);
        }

        [Fact]
        public void FindCandidates_IsDeliberatelyLoose_AndReturnsWordsThatAreNotCorrections()
        {
            // Arrange
            // "rame" and "rain" both reduce to "ra" after two deletions, so the index
            // offers "rain" even though it is four edits away. Filtering that out is
            // EditModel's job, not the index's. This test pins the contract.
            WordDictionary dictionary = new WordDictionary(new[] { "rain" });

            // Act
            List<int> candidates = dictionary.FindCandidates("rame").ToList();

            // Assert
            Assert.Contains(0, candidates);
        }

        [Fact]
        public void FindCandidates_WordWithNothingInCommon_ReturnsNothing()
        {
            // Arrange
            WordDictionary dictionary = new WordDictionary(new[] { "rain" });

            // Act
            List<int> candidates = dictionary.FindCandidates("zzzzzz").ToList();

            // Assert
            Assert.Empty(candidates);
        }
    }
}