using SpellCheckerApp.Application.Contracts;

namespace SpellCheckerApp.Cli;

/// <summary>Adapts any TextReader to the application's input.</summary>
public sealed class TextReaderLineAdapter : ILineSource
{
    private readonly TextReader _reader;

    public TextReaderLineAdapter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _reader = reader;
    }

    public string? ReadLine()
    {
        return _reader.ReadLine();
    }
}
