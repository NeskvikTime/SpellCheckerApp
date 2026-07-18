using SpellCheckerApp.Application.Contracts;
using SpellCheckerApp.Domain.Correction;
using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Application;

/// <summary>
/// The use case: read a document, print it corrected. Owns the order of events and
/// nothing else — every decision it makes is delegated.
/// </summary>
public class SpellCheckDocumentHandler
{
    public void Handle(ILineSource source, ILineSink sink)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(sink);

        // The dictionary arrives as part of the request, so the checker can only be
        // built once we have read it. DictionaryReader validates word length as it
        // reads, so anything past this point is known to be within the limit.
        List<string> words = DictionaryReader.Read(source);
        var dictionary = new WordDictionary(words);
        ISpellChecker checker = new CachingSpellChecker(new SpellChecker(dictionary));

        string? line;
        while ((line = source.ReadLine()) != null)
        {
            if (SectionSeparator.Matches(line))
            {
                break;
            }

            CorrectLine(line, checker, sink);
        }
    }

    private static void CorrectLine(string line, ISpellChecker checker, ILineSink sink)
    {
        foreach (TextSegment segment in WordTokenizer.Split(line))
        {
            if (!segment.IsWord)
            {
                sink.Write(segment.Value);
                continue;
            }

            // The spec's 50-character limit applies to text words too ("both").
            WordLengthLimit.Validate(segment.Value);

            CorrectionResult result = checker.Check(segment.Value);
            sink.Write(CorrectionResultFormatter.Format(segment.Value, result));
        }

        sink.WriteLine();
    }
}
