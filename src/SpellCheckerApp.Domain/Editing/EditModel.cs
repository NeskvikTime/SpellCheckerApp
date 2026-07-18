namespace SpellCheckerApp.Domain.Editing;

/// <summary>
/// The rules of the problem, and the only place they live.
///
///   edit := insert one letter | delete one letter
///   If both edits are insertions, or both are deletions, they may not be of
///   adjacent characters.
///
/// Because every edit changes the length by exactly one, the number of edits is
/// fully determined by how the two lengths compare. Reading the table below:
/// the left column is (candidate.Length - text.Length), the right column is the
/// number of edits that implies.
///
///   candidate is 2 shorter -> 2 edits: two deletions from the text word
///                                      (same kind, so they must not be adjacent)
///   candidate is 1 shorter -> 1 edit:  one deletion from the text word
///   same length            -> 0 edits if the words are equal, otherwise
///                             2 edits: one deletion plus one insertion
///                             (mixed kinds, so the restriction does not apply)
///   candidate is 1 longer  -> 1 edit:  one insertion into the text word
///   candidate is 2 longer  -> 2 edits: two insertions into the text word
///                                      (same kind, so they must not be adjacent)
///
/// Any other difference in length needs at least three edits, so it is out of range.
/// </summary>
public static class EditModel
{
    public const int Unreachable = -1;

    /// <summary>
    /// Number of edits turning <paramref name="text"/> into <paramref name="candidate"/>,
    /// or <see cref="Unreachable"/> if that is not possible within two legal edits.
    /// Both arguments are expected to be keys (see WordKey).
    /// </summary>
    public static int? Distance(string text, string candidate)
    {
        int lengthDifference = candidate.Length - text.Length;

        switch (lengthDifference)
        {
            case 0:
                if (text == candidate)
                {
                    return 0;
                }

                return CanMoveOneLetter(text, candidate) ? 2 : (int?)null;

            case 1:
                return HasDeletion(candidate, text, 0) ? 1 : (int?)null;

            case -1:
                return HasDeletion(text, candidate, 0) ? 1 : (int?)null;

            case 2:
                return CanDeleteTwoNonAdjacent(candidate, text) ? 2 : (int?)null;

            case -2:
                return CanDeleteTwoNonAdjacent(text, candidate) ? 2 : (int?)null;

            default:
                return null;
        }
    }

    /// <summary>
    /// Both words have the same length but differ. Reachable in two edits if one
    /// letter can be deleted and another inserted. Only two shapes need testing:
    /// move the letter at the first difference to the last difference, or the other
    /// way round. A plain substitution is the case where the two positions coincide.
    /// </summary>
    private static bool CanMoveOneLetter(string a, string b)
    {
        int first = CommonPrefixLength(a, b);
        int last = a.Length - 1 - CommonSuffixLength(a, b);
        int length = last - first;

        if (a.Substring(first + 1, length) == b.Substring(first, length))
        {
            return true;
        }

        if (a.Substring(first, length) == b.Substring(first + 1, length))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// "longer" is exactly two letters longer than "shorter". Returns true if two
    /// letters at non-adjacent positions can be removed from "longer" to produce
    /// "shorter". Repeated letters mean several removal pairs may give the same
    /// result; one legal (non-adjacent) pair is enough.
    /// </summary>
    private static bool CanDeleteTwoNonAdjacent(string longer, string shorter)
    {
        // The lower of the two removed positions can never sit past the first
        // difference, otherwise the prefix would no longer match.
        int maxFirst = CommonPrefixLength(longer, shorter);

        for (int i = 0; i <= maxFirst; i++)
        {
            string reduced = longer.Remove(i, 1);

            // Position i + 1 in "reduced" is position i + 2 in "longer",
            // which is what keeps the two removals non-adjacent.
            if (HasDeletion(reduced, shorter, i + 1))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// "longer" is exactly one letter longer than "shorter". The positions whose
    /// removal turns one into the other always form a contiguous range:
    /// [shorter.Length - commonSuffix, commonPrefix]. Returns true if that range
    /// contains any position at or after "minIndex".
    /// </summary>
    private static bool HasDeletion(string longer, string shorter, int minIndex)
    {
        int prefix = CommonPrefixLength(longer, shorter);
        int suffix = CommonSuffixLength(longer, shorter);

        int lowest = shorter.Length - suffix;
        if (lowest < minIndex)
        {
            lowest = minIndex;
        }

        int highest = prefix;

        return lowest <= highest;
    }

    private static int CommonPrefixLength(string a, string b)
    {
        int max = Math.Min(a.Length, b.Length);
        int i = 0;
        while (i < max && a[i] == b[i])
        {
            i++;
        }

        return i;
    }

    private static int CommonSuffixLength(string a, string b)
    {
        int max = Math.Min(a.Length, b.Length);
        int i = 0;
        while (i < max && a[a.Length - 1 - i] == b[b.Length - 1 - i])
        {
            i++;
        }

        return i;
    }
}