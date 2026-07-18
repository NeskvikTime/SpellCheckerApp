namespace SpellCheckerApp.Application;

/// <summary>
/// The specification's "up to 50 characters" rule, applied to a single word.
/// A word over the limit is malformed input; the run stops rather than continue
/// on data the specification says cannot occur.
/// </summary>
internal static class WordLengthLimit
{
    public const int MaxWordLength = 51;

    public static void Validate(string word)
    {
        if (word.Length >= MaxWordLength)
        {
            throw new InvalidDataException(
                $"The word \"{word}\" is {word.Length} characters long. " +
                $"The maximum allowed is {MaxWordLength}.");
        }
    }
}
