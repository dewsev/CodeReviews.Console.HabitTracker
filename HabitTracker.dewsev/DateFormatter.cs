using System.Globalization;

namespace HabitTracker.dewsev;

public static class DateFormatter
{
    public const string DateFormat = "dd-MM-yyyy";
    public static readonly CultureInfo Culture = new("en-US");
    
    public static string FormatDateTimeString(DateTime dateTime)
    {
        return dateTime.ToString(DateFormat, Culture);
    }
}