namespace SpellCheckerApp.Domain.Text;

/// <summary>
/// "The input is case-insensitive." This is that rule, in one place.
/// A key is the form of a word used for comparison and indexing; the original
/// casing is never lost, it is simply carried separately.
/// </summary>
public static class WordKey
{
    public static string For(string word)
    {
        return word.ToLowerInvariant();
    }
}
