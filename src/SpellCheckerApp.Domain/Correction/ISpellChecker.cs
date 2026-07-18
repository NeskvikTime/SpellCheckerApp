namespace SpellCheckerApp.Domain.Correction;

/// <summary>
/// Decides what should happen to a single word. Implementations take the word as
/// it was written; normalisation is their business, not the caller's.
/// </summary>
public interface ISpellChecker
{
    CorrectionResult Check(string word);
}
