using System.Security.Cryptography;

namespace TaskDeck.Common.Utils;

/// <summary>
/// Utility methods for string operations
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Truncate a string to a specified length
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= maxLength) return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Generate a random string of specified length
    /// </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = RandomNumberGenerator.GetBytes(length);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }

    /// <summary>
    /// Convert a string to slug format
    /// </summary>
    public static string ToSlug(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        return value
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate("", (current, c) => current + c);
    }
}
