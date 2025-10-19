using System.Text;
using System.Text.Json;

namespace CourtListener.MCP.Server.Configuration;

/// <summary>
/// JSON naming policy that converts PascalCase to snake_case.
/// Example: "AbsoluteUrl" -> "absolute_url"
/// </summary>
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var builder = new StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];

            if (char.IsUpper(c))
            {
                // Add underscore before uppercase letter (except for first character)
                if (i > 0)
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
