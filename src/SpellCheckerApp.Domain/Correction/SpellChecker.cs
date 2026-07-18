using SpellCheckerApp.Domain.Editing;
using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Domain.Correction;

/// <summary>
/// The selection rules from the specification:
///   - a word in the dictionary stands as written;
///   - otherwise gather everything within two legal edits;
///   - if anything is one edit away, discard everything two edits away;
///   - what is left is the answer, in dictionary order.
/// </summary>
public class SpellChecker : ISpellChecker
{
    private readonly WordDictionary _dictionary;

    public SpellChecker(WordDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        _dictionary = dictionary;
    }

    public CorrectionResult Check(string word)
    {
        string key = WordKey.For(word);

        if (_dictionary.Contains(key))
        {
            return CorrectionResult.Known;
        }

        List<int> oneEdit = [];
        List<int> twoEdits = [];

        foreach (int id in _dictionary.FindCandidates(key))
        {
            // The index over-generates and cannot see the adjacency rule.
            // This is where a candidate becomes a correction, or does not at all.
            // Null means "no allowed path within two edits" - drops it.
            int? distance = EditModel.Distance(key, _dictionary.GetKey(id));

            if (distance == 1)
            {
                oneEdit.Add(id);
            }
            else if (distance == 2)
            {
                twoEdits.Add(id);
            }
        }

        List<int> chosen = oneEdit.Count > 0 ? oneEdit : twoEdits;

        if (chosen.Count == 0)
        {
            return CorrectionResult.Unknown;
        }

        chosen.Sort();

        var suggestions = new List<string>(chosen.Count);
        foreach (int id in chosen)
        {
            suggestions.Add(_dictionary.GetWord(id));
        }

        return CorrectionResult.FromSuggestions(suggestions);
    }
}
