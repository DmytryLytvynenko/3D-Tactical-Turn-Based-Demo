using System;

public static class StringExtensions
{
    /// <summary>
    /// Converts a string to an enum value of type T. Returns a default value if parsing fails.
    /// </summary>
    /// <typeparam name="T">The enum type to convert to.</typeparam>
    /// <param name="value">The string value to convert.</param>
    /// <param name="defaultValue">The default value to return if parsing fails.</param>
    /// <returns>The parsed enum value or the default value.</returns>
    public static T ToEnum<T>(this string value, T defaultValue) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        return Enum.TryParse(value, true, out T result) ? result : defaultValue;
    }
}

