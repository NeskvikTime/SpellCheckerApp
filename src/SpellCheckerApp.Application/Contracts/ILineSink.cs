namespace SpellCheckerApp.Application.Contracts;

/// <summary>
/// Somewhere to put the corrected text. Deliberately narrower than TextWriter.
/// </summary>
public interface ILineSink
{
    void Write(string text);

    void WriteLine();
}
