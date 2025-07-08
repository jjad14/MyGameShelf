using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Helpers;
public static class StringHelpers
{
    /// <summary>
    /// Converts a string to kebab-case (lowercase with hyphens).
    /// Example: "Hello World" => "hello-world"
    /// </summary>
    public static string ToKebabCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Trim, lowercase, replace spaces with hyphens
        return input.Trim().ToLowerInvariant().Replace(' ', '-');
    }

    // Add more string helper methods here as needed

    /// <summary>
    /// Converts a string to snake_case (lowercase with underscores).
    /// Example: "Hello World" => "hello_world"
    /// </summary>
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input.Trim().ToLowerInvariant().Replace(' ', '_');
    }

    /// <summary>
    /// Capitalizes the first letter of the string.
    /// Example: "hello" => "Hello"
    /// </summary>
    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
