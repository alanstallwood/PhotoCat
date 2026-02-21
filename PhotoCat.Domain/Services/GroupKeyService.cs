using System.Text.RegularExpressions;

namespace PhotoCat.Domain.Services;

public static partial class GroupKeyService
{
    private static readonly string[] Modifiers =
    {
        "edit", "edited", "crop", "cropped",
        "final", "copy", "bw", "hdr",
        "small", "large", "export"
    };

    public static string GetBestGroupKey(string currentGroupKey, string newFileName)
    {
        var candidate = NormalizeAndRemoveModifiers(newFileName);

        if (string.IsNullOrWhiteSpace(currentGroupKey))
        {
            return candidate;
        }

        var currentMatch = StructuredPattern().Match(currentGroupKey);
        var candidateMatch = StructuredPattern().Match(candidate);

        // If both are structured camera-style names
        if (currentMatch.Success && candidateMatch.Success)
        {
            var currentPrefix = currentMatch.Groups[1].Value;
            var currentNumber = currentMatch.Groups[2].Value;

            var candidatePrefix = candidateMatch.Groups[1].Value;
            var candidateNumber = candidateMatch.Groups[2].Value;

            // Different numeric identity → NEVER evolve
            if (currentPrefix == candidatePrefix &&
                currentNumber != candidateNumber)
            {
                return currentGroupKey;
            }
        }

        var currentScore = Score(currentGroupKey);
        var candidateScore = Score(candidate);

        return candidateScore > currentScore
            ? candidate
            : currentGroupKey;
    }

    private static int Score(string key)
    {
        int score = 0;

        if (StructuredPattern().IsMatch(key))
            score += 100;

        score += Math.Max(0, 50 - key.Length);

        return score;
    }

    private static string NormalizeAndRemoveModifiers(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName)
            .ToLowerInvariant();

        name = NonAlphaNumeric().Replace(name, "");

        foreach (var mod in Modifiers)
            name = name.Replace(mod, "");

        return name;
    }

    [GeneratedRegex(@"^([a-z]+)(\d+)$", RegexOptions.Compiled)]
    private static partial Regex StructuredPattern();
    [GeneratedRegex(@"[^a-z0-9]")]
    private static partial Regex NonAlphaNumeric();
}
