using System.Text;
using SpellCheckerApp.Application;

namespace SpellCheckerApp.Cli;

/// <summary>
/// Runs the use case over two files. Knows about streams, encodings and append
/// mode. Knows nothing about where the paths came from or what to do if something
/// goes wrong - that stays in Program.
/// </summary>
public sealed class FileSpellCheckRunner
{
    private const int BufferSize = 65536;

    public void Check(string inputPath, string outputPath)
    {
        // A run is separated from the previous one by a blank line - but only when
        // there is a previous one. This is decided before the writer opens, because
        // once we append there is no way to tell what the file held before.
        bool needsSeparator = File.Exists(outputPath) && new FileInfo(outputPath).Length > 0;

        // detectEncodingFromByteOrderMarks: true, so a file saved with a BOM still reads.
        using var reader = new StreamReader(inputPath, Encoding.UTF8, true, BufferSize);

        // append: true -> creates the file if missing, otherwise adds to the end.
        // UTF8Encoding(false) -> never writes a BOM. With append that matters: a BOM
        // would land mid-file on the second run and corrupt the line before it.
        using var writer = new StreamWriter(outputPath, append: true, new UTF8Encoding(false), BufferSize);

        if (needsSeparator)
        {
            writer.Write('\n');
        }

        var handler = new SpellCheckDocumentHandler();
        handler.Handle(new TextReaderLineAdapter(reader), new TextWriterLineAdapter(writer));

        writer.Flush();
    }
}
