namespace SpellCheckerApp.Domain.Text;

/// <summary>
/// "Words (strings of letters)." A word is a maximal run of letters;
/// anything else is a gap.
/// </summary>
public static class WordTokenizer
{
    public static IEnumerable<TextSegment> Split(string line)
    {
        int i = 0;

        while (i < line.Length)
        {
            bool isWord = char.IsLetter(line[i]);
            int start = i;

            while (i < line.Length && char.IsLetter(line[i]) == isWord)
            {
                i++;
            }

            yield return new TextSegment(line.Substring(start, i - start), isWord);
        }
    }

    public static IEnumerable<string> Words(string line)
    {
        foreach (TextSegment segment in Split(line))
        {
            if (segment.IsWord)
            {
                yield return segment.Value;
            }
        }
    }
}
