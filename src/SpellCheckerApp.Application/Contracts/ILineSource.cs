namespace SpellCheckerApp.Application.Contracts;

/// <summary>
/// A stream of lines. The Application drives this; it does not care whether the
/// lines come from a console, a file, or a test fixture.
/// </summary>
public interface ILineSource
{
    /// <summary>The next line, or null once there are none left.</summary>
    string? ReadLine();
}
