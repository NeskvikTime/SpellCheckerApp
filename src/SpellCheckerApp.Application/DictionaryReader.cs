using SpellCheckerApp.Application.Contracts;
using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Application;

/// <summary>
/// Reads the dictionary section: free format, any number of lines, terminated by
/// the separator (===). Deduplication is not done here; the dictionary owns that rule.
/// </summary>
public static class DictionaryReader
{
    public static List<string> Read(ILineSource source)
    {
        var words = new List<string>();

        string? line;
        while ((line = source.ReadLine()) != null)
        {
            if (SectionSeparator.Matches(line))
            {
                break;
            }

            foreach (string word in WordTokenizer.Words(line))
            {
                WordLengthLimit.Validate(word);
                words.Add(word);
            }
        }

        return words;
    }
}
