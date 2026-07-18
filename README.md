# Spell Checker

A console application that corrects words against a dictionary, per the CQG spell-checker
assignment: a word is corrected if it is reachable from a dictionary word within two edits
(insert or delete a single letter), with the restriction that two edits of the same kind
(both insertions or both deletions) may not touch adjacent characters.

## Project layout

- `src/SpellCheckerApp.Domain` — the correction rules themselves: edit distance, the
  dictionary index, word normalisation. No I/O.
- `src/SpellCheckerApp.Application` — the use case (read a document, print it corrected) and
  the file-format rules (`===` separators, the 50-character word limit). No console/file I/O.
- `src/SpellCheckerApp.Console` — the executable: command-line handling and file I/O.
- `tests/` — one xUnit project per `src` project above.
- `results/` — a real input/output pair, produced by actually running the console app (see
  below), for a reviewer to inspect without building anything.

## Building

In order to build solution, you need to navigate to the src\SpellCheckerApp.Console\ directory and run the following command:

```
dotnet build src\SpellCheckerApp.Console\SpellCheckerApp.Console.csproj
```

This also builds `SpellCheckerApp.Domain` and `SpellCheckerApp.Application`, since the console
project references both.

## Running

In order to run solution, you need to navigate to the src\SpellCheckerApp.Console\ directory and run the following command:

```
dotnet run --project src\SpellCheckerApp.Console\SpellCheckerApp.Console.csproj -- <file>
```

`<file>` is optional and is interpreted as follows:

- **No argument**: defaults to `C:\Temp\input.txt`. If that file doesn't exist, an empty one
  is created there and the program exits, so you can fill it in and run again.
- **A bare file name** (e.g. `test.txt`): resolved against `C:\Temp`, i.e. `C:\Temp\test.txt`.
- **A full or relative path**: used as given; `C:\Temp` is not involved.

Only `.txt` files are accepted.

The input file has the format described in the assignment: a dictionary section, a line
containing just `===`, zero or more text lines to correct, and a final `===` line.

### Output

Results are written next to the input file, with `_out` inserted before the extension:
`C:\Temp\input.txt` → `C:\Temp\input_out.txt`.

**Output is appended, not overwritten.** Running the same input twice adds a second copy of
the results to the end of the output file rather than replacing the first. Delete or rename
the output file first if you want a clean one.

## Testing

Each test project can be run independently:

```
dotnet test tests\SpellCheckerApp.Domain.Tests\SpellCheckerApp.Domain.Tests.csproj
dotnet test tests\SpellCHeckerApp.Application.Tests\SpellCheckerApp.Application.Tests.csproj
dotnet test tests\SpellCheckerApp.Console.Tests\SpellCheckerApp.Console.Tests.csproj
```

## Samples

`samples/input.txt` is the worked example from the assignment; `samples/input_out.txt` is the
actual output of running it through this console app — not hand-written. It matches the
assignment's expected output exactly.
