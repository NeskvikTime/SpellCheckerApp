using SpellCheckerApp.Domain.Editing;
using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Domain.Correction;

/// <summary>
/// The dictionary: a list of words in the order they were read, plus a fast way
/// to find words that might be close to some other word.
///
/// The order matters, because corrections have to be printed in dictionary order.
/// So each word keeps its place and gets an id: 0, 1, 2, and so on.
///
/// The same word twice, in any casing, is stored once. The first spelling wins.
///
/// FindCandidates gives only intermediate (approximate) results. It gives back words that might be
/// corrections. Some of them will not be. It also cannot check the "no adjacent
/// letters" rule, because it only remembers the shortened strings, not which
/// letters were removed to get them. So always check each candidate with EditModel.
/// </summary>
public sealed class WordDictionary
{
    private const int MaxEdits = 2;

    private readonly List<string> _words = [];
    private readonly List<string> _keys = [];

    private readonly Dictionary<string, int> _known = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<int>> _variants = new(StringComparer.Ordinal);

    public WordDictionary(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        foreach (string word in words)
        {
            RegisterWord(word);
        }
    }

    public int Count
    {
        get { return _words.Count; }
    }

    public bool Contains(string key)
    {
        return _known.ContainsKey(key);
    }

    /// <summary>Word as it was written in the dictionary.</summary>
    public string GetWord(int id)
    {
        return _words[id];
    }

    /// <summary>Word normalised for comparison.</summary>
    public string GetKey(int id)
    {
        return _keys[id];
    }

    /// <summary>
    /// Ids of words that share a shortened form with the given key. Always includes
    /// every real match, but also includes words that are not matches at all.
    /// </summary>
    public IEnumerable<int> FindCandidates(string key)
    {
        var candidates = new HashSet<int>();

        foreach (string variant in DeletionVariants.Generate(key, MaxEdits))
        {
            List<int>? ids;
            if (_variants.TryGetValue(variant, out ids))
            {
                candidates.UnionWith(ids);
            }
        }

        return candidates;
    }

    private void RegisterWord(string word)
    {
        string key = WordKey.For(word);

        if (_known.ContainsKey(key))
        {
            return;
        }

        int id = _words.Count;
        _words.Add(word);
        _keys.Add(key);
        _known.Add(key, id);

        foreach (string variant in DeletionVariants.Generate(key, MaxEdits))
        {
            List<int>? ids;
            if (!_variants.TryGetValue(variant, out ids))
            {
                ids = [];
                _variants.Add(variant, ids);
            }

            // Ids arrive in increasing order, so every list stays sorted by
            // position in the dictionary for free.
            ids.Add(id);
        }
    }

}
