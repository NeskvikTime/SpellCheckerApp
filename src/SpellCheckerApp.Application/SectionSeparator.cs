namespace SpellCheckerApp.Application;

/// <summary>
/// The shape of the input document: a dictionary, a separator, text lines, a
/// separator. This is a format concern, not a domain rule, so it lives here.
/// </summary>
internal static class SectionSeparator
{
    public const string Separator = "===";

    public static bool Matches(string line)
    {
        return line.Trim() == Separator;
    }
}
