namespace SpellCheckerApp.Domain.Text;

/// <summary>
/// A run of characters from a line: either a word or the gap between words.
/// Gaps are carried through untouched, which is how "whitespace intact" is honoured.
/// </summary>
public struct TextSegment
{
    public readonly string Value;
    public readonly bool IsWord;

    public TextSegment(string value, bool isWord)
    {
        Value = value;
        IsWord = isWord;
    }
}
