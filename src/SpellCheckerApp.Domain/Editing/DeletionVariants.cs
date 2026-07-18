namespace SpellCheckerApp.Domain.Editing;

/// <summary>
/// Every string reachable by removing up to maxDeletions letters, the word itself
/// included. These are the index keys.
///
/// This works only because an edit is an insert or a delete and nothing else: any
/// sequence of such edits can be reordered so all deletions happen first, which
/// means two words within N edits always share a variant at some split k + j = N.
/// </summary>
public static class DeletionVariants
{
    public static IEnumerable<string> Generate(string word, int maxDeletions)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal)
        {
            word
        };

        yield return word;

        var frontier = new List<string>
        {
            word
        };

        for (int level = 0; level < maxDeletions; level++)
        {
            var next = new List<string>();

            foreach (string source in frontier)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    string variant = source.Remove(i, 1);

                    if (!seen.Add(variant))
                    {
                        continue;
                    }

                    next.Add(variant);
                    yield return variant;
                }
            }

            frontier = next;
        }
    }
}
