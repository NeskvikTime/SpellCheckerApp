using SpellCheckerApp.Domain.Correction;
using SpellCheckerApp.Domain.Text;

namespace SpellCheckerApp.Application;

/// <summary>
/// Memoises an ISpellChecker. Words in real text follow a Zipf distribution, so a
/// handful of them account for most of the work; resolving "the" five thousand
/// times is the single easiest thing to stop doing.
///
/// This is a decorator rather than a feature of the domain checker because caching
/// is a performance decision, not a rule of the problem.
/// </summary>
public sealed class CachingSpellChecker : ISpellChecker
{
    private readonly ISpellChecker _inner;
    private readonly Dictionary<string, CorrectionResult> _cache = new(StringComparer.Ordinal);

    public CachingSpellChecker(ISpellChecker inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
    }

    public CorrectionResult Check(string word)
    {
        string key = WordKey.For(word);

        CorrectionResult? result;
        if (!_cache.TryGetValue(key, out result))
        {
            result = _inner.Check(word);
            _cache.Add(key, result);
        }

        return result;
    }
}
