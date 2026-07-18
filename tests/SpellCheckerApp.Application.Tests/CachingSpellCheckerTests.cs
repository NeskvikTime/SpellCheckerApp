using NSubstitute;
using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Application.Tests
{
    /// <summary>
    /// The only thing this class promises is "do not ask the inner checker twice".
    /// That is a statement about a call, not about a return value, so a substitute
    /// is the right tool here rather than a real SpellChecker.
    /// </summary>
    public class CachingSpellCheckerTests
    {
        [Fact]
        public void Check_SameWordTwice_AsksTheInnerCheckerOnlyOnce()
        {
            // Arrange
            ISpellChecker inner = Substitute.For<ISpellChecker>();
            inner.Check(Arg.Any<string>()).Returns(CorrectionResult.Known);
            CachingSpellChecker sut = new CachingSpellChecker(inner);

            // Act
            sut.Check("the");
            sut.Check("the");

            // Assert
            inner.Received(1).Check(Arg.Any<string>());
        }

        [Fact]
        public void Check_SameWordInADifferentCase_IsTreatedAsTheSameWord()
        {
            // Arrange
            ISpellChecker inner = Substitute.For<ISpellChecker>();
            inner.Check(Arg.Any<string>()).Returns(CorrectionResult.Known);
            CachingSpellChecker sut = new CachingSpellChecker(inner);

            // Act
            sut.Check("The");
            sut.Check("the");
            sut.Check("THE");

            // Assert
            inner.Received(1).Check(Arg.Any<string>());
        }

        [Fact]
        public void Check_DifferentWords_AsksTheInnerCheckerForEachOne()
        {
            // Arrange
            ISpellChecker inner = Substitute.For<ISpellChecker>();
            inner.Check(Arg.Any<string>()).Returns(CorrectionResult.Known);
            CachingSpellChecker sut = new CachingSpellChecker(inner);

            // Act
            sut.Check("the");
            sut.Check("in");

            // Assert
            inner.Received(2).Check(Arg.Any<string>());
        }

        [Fact]
        public void Check_ReturnsWhateverTheInnerCheckerSaid()
        {
            // Arrange
            CorrectionResult expected = CorrectionResult.FromSuggestions(new[] { "main" });
            ISpellChecker inner = Substitute.For<ISpellChecker>();
            inner.Check("mainy").Returns(expected);
            CachingSpellChecker sut = new CachingSpellChecker(inner);

            // Act
            CorrectionResult result = sut.Check("mainy");

            // Assert
            Assert.Same(expected, result);
        }

        [Fact]
        public void Check_CachedCall_ReturnsTheSameResultAsTheFirstCall()
        {
            // Arrange
            CorrectionResult expected = CorrectionResult.FromSuggestions(new[] { "main" });
            ISpellChecker inner = Substitute.For<ISpellChecker>();
            inner.Check(Arg.Any<string>()).Returns(expected);
            CachingSpellChecker sut = new CachingSpellChecker(inner);

            // Act
            CorrectionResult first = sut.Check("mainy");
            CorrectionResult second = sut.Check("mainy");

            // Assert
            Assert.Same(first, second);
        }

        [Fact]
        public void Constructor_NullInner_Throws()
        {
            // Arrange
            ISpellChecker inner = null;

            // Act
            Action act = () => new CachingSpellChecker(inner);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}