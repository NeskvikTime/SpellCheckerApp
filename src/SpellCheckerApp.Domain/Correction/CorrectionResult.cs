using System.Collections.ObjectModel;

namespace SpellCheckerApp.Domain.Correction;

/// <summary>
/// The verdict on a single word. Carries the suggestions themselves, in dictionary
/// order and dictionary casing, but says nothing about how they should be printed.
/// Formatting is not a domain concern.
/// </summary>
public sealed class CorrectionResult
{
    private static readonly IList<string> None = new ReadOnlyCollection<string>(new List<string>());

    public static readonly CorrectionResult Known = new CorrectionResult(CorrectionKind.Known, None);
    public static readonly CorrectionResult Unknown = new CorrectionResult(CorrectionKind.Unknown, None);

    private CorrectionResult(CorrectionKind kind, IList<string> suggestions)
    {
        Kind = kind;
        Suggestions = suggestions;
    }

    public CorrectionKind Kind { get; private set; }

    /// <summary>Empty unless Kind is Corrected.</summary>
    public IList<string> Suggestions { get; private set; }

    public static CorrectionResult FromSuggestions(IList<string> suggestions)
    {
        ArgumentNullException.ThrowIfNull(suggestions);

        if (suggestions.Count == 0)
        {
            throw new ArgumentException("A correction must have at least one suggestion.", nameof(suggestions));
        }

        return new CorrectionResult(CorrectionKind.Corrected, new ReadOnlyCollection<string>(suggestions));
    }
}