using System.Globalization;

namespace HabitTracker.dewsev;

public static class DateParser
{
    public const string DateFormat = "dd/MM/yyyy";
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    
    public static string GetDateTimeString(DateTime dateTime)
    {
        return dateTime.ToString(DateFormat, Culture);
    }
    
    public static DateTime ParseDateTimeString(string dateTimeString)
    {
        return DateTime.ParseExact(dateTimeString, DateFormat, Culture);
    }

    public static bool IsValidDateFormat(string? dateString)
    {
        return DateTime.TryParseExact(dateString, DateFormat, Culture, DateTimeStyles.None, out _);
    }
}