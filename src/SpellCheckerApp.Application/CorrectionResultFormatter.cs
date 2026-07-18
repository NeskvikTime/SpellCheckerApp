using SpellCheckerApp.Domain.Correction;

namespace SpellCheckerApp.Application;

/// <summary>
/// Turns a verdict into the text the specification asks for. The domain decides
/// what the corrections are, this decides what the result with braces would look like.
/// </summary>
public static class CorrectionResultFormatter
{
    public static string Format(string original, CorrectionResult result)
    {
        switch (result.Kind)
        {
            case CorrectionKind.Known:
                // Casing comes from the text.
                return original;

            case CorrectionKind.Corrected:

                // If only one word is suggested, then is written in the result as is with out braces.
                if (result.Suggestions.Count == 1)
                {
                    return result.Suggestions[0];
                }

                // Writes suggestions, example : {main mainly}
                return "{" + string.Join(" ", result.Suggestions) + "}";

            // Writes original word, example : {rame?}
            default:
                return "{" + original + "?}";
        }
    }
}
