using System.Globalization;
using System.Text;

namespace ExamenTec.Infrastructure.Helpers;

public static class StringHelper
{
    public static string NormalizeForSearch(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();
    }
}

