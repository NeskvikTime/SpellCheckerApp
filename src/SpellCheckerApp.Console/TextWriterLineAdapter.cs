using SpellCheckerApp.Application.Contracts;

namespace SpellCheckerApp.Cli;

/// <summary>Adapts any TextWriter to the application's output.</summary>
public sealed class TextWriterLineAdapter : ILineSink
{
    private readonly TextWriter _writer;

    public TextWriterLineAdapter(TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        _writer = writer;
    }

    public void Write(string text)
    {
        _writer.Write(text);
    }

    public void WriteLine()
    {
        _writer.Write('\n');
    }
}
