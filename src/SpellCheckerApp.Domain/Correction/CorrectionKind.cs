namespace SpellCheckerApp.Domain.Correction;

public enum CorrectionKind
{
    /// <summary>The word is in the dictionary and stands as written.</summary>
    Known,

    /// <summary>The word is not in the dictionary and nothing is close enough.</summary>
    Unknown,

    /// <summary>The word is not in the dictionary but one or more corrections exist.</summary>
    Corrected
}
