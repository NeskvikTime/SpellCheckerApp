using System.Text;
using NSubstitute;
using SpellCheckerApp.Application.Contracts;

namespace SpellCheckerApp.Application.Tests
{
    public class SpellCheckDocumentHandlerTests
    {
        /// <summary>
        /// A substitute that records everything written, so a test can assert on the
        /// finished text rather than on a sequence of calls.
        /// </summary>
        private static ILineSink CreateRecordingSink(StringBuilder recorder)
        {
            ILineSink sink = Substitute.For<ILineSink>();
            sink.When(s => s.Write(Arg.Any<string>())).Do(call => recorder.Append(call.Arg<string>()));
            sink.When(s => s.WriteLine()).Do(call => recorder.Append('\n'));
            return sink;
        }

        [Fact]
        public void Handle_TheExampleFromTheSpecification_ProducesTheExpectedOutput()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns(
                "rain spain plain plaint pain main mainly",
                "the in on fall falls his was",
                "===",
                "hte rame in pain fells",
                "mainy oon teh lain",
                "was hints pliant",
                "===",
                null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            string expected =
                "the {rame?} in pain falls\n" +
                "{main mainly} on the plain\n" +
                "was {hints?} plaint\n";

            Assert.Equal(expected, written.ToString());
        }

        [Fact]
        public void Handle_LeavesWhitespaceExactlyAsItFoundIt()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the in", "===", "  the   in  ", "===", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal("  the   in  \n", written.ToString());
        }

        [Fact]
        public void Handle_StopsAtTheSecondSeparator()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the", "===", "the", "===", "the", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal("the\n", written.ToString());
        }

        [Fact]
        public void Handle_NoTextLines_WritesNothing()
        {
            // Arrange
            // "there will be zero or more text lines"
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the in", "===", "===", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal(string.Empty, written.ToString());
        }

        [Fact]
        public void Handle_BlankTextLine_StillEmitsTheLine()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the", "===", "", "the", "===", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal("\nthe\n", written.ToString());
        }

        [Fact]
        public void Handle_TextRunsOutWithoutASecondSeparator_StillCorrectsWhatItRead()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the", "===", "hte", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal("the\n", written.ToString());
        }

        [Fact]
        public void Handle_RepeatedWords_AreOnlyResolvedOnce()
        {
            // Arrange
            // Not a behaviour the caller can see, so this asserts through the output:
            // caching must not change what gets printed.
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the", "===", "hte hte hte", "===", null);

            StringBuilder written = new StringBuilder();
            ILineSink sink = CreateRecordingSink(written);
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            sut.Handle(source, sink);

            // Assert
            Assert.Equal("the the the\n", written.ToString());
        }

        [Fact]
        public void Handle_TextWordOverTheLimit_Throws()
        {
            // Arrange
            // The spec's "up to 50 characters" rule applies to text-line words too,
            // not just the dictionary (SpellCheckDocumentHandler.cs enforces this itself).
            string tooLong = new string('a', WordLengthLimit.MaxWordLength);
            ILineSource source = Substitute.For<ILineSource>();
            source.ReadLine().Returns("the", "===", tooLong, "===", null);

            ILineSink sink = Substitute.For<ILineSink>();
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            Action act = () => sut.Handle(source, sink);

            // Assert
            Assert.Throws<InvalidDataException>(act);
        }

        [Fact]
        public void Handle_NullSource_Throws()
        {
            // Arrange
            ILineSink sink = Substitute.For<ILineSink>();
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            Action act = () => sut.Handle(null!, sink);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Handle_NullSink_Throws()
        {
            // Arrange
            ILineSource source = Substitute.For<ILineSource>();
            SpellCheckDocumentHandler sut = new SpellCheckDocumentHandler();

            // Act
            void act() => sut.Handle(source, null!);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}