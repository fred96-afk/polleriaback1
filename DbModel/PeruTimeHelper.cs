using System;

namespace DbModel;

public static class PeruTimeHelper
{
    private static readonly TimeZoneInfo PeruTimeZone = ResolvePeruTimeZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PeruTimeZone);

    private static TimeZoneInfo ResolvePeruTimeZone()
    {
        var candidates = new[]
        {
            "SA Pacific Standard Time", // Windows
            "America/Lima" // Linux/macOS
        };

        foreach (var candidate in candidates)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(candidate);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        // Fallback to UTC-5 if timezone not found
        return TimeZoneInfo.CreateCustomTimeZone("Peru", TimeSpan.FromHours(-5), "Peru", "Peru");
    }
}
