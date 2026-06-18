using AuthService.Errors;

namespace AuthService.Lib.Utils;

public static class UuidParser
{
    public static bool TryParse(
        string? raw,
        out Guid uuid,
        out ApiError? error,
        string fieldName)
    {
        uuid = default;
        error = null;

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = CreateInvalidFormatError(fieldName, [raw ?? string.Empty]);
            return false;
        }

        if (Guid.TryParse(raw.Trim(), out uuid))
        {
            return true;
        }

        error = CreateInvalidFormatError(fieldName, [raw]);
        return false;
    }

    public static bool TryParseDistinct(
        string[]? rawValues,
        out Guid[] uuids,
        out ApiError? error,
        string fieldName)
    {
        uuids = [];
        error = null;

        if (rawValues == null || rawValues.Length == 0)
        {
            return true;
        }

        List<Guid> parsed = [];
        List<string> invalid = [];

        foreach (string raw in rawValues.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(raw) || !Guid.TryParse(raw.Trim(), out Guid uuid))
            {
                invalid.Add(raw);
                continue;
            }

            parsed.Add(uuid);
        }

        if (invalid.Count > 0)
        {
            error = CreateInvalidFormatError(fieldName, invalid);
            return false;
        }

        uuids = parsed.ToArray();
        return true;
    }

    private static ApiError CreateInvalidFormatError(string fieldName, IEnumerable<string> invalidValues) =>
        new()
        {
            StatusCode = "0.2.2",
            Title = "Неверный запрос",
            Message = $"{fieldName} должно быть в формате UUID",
            Field = fieldName,
            Details = string.Join(", ", invalidValues),
        };
}
