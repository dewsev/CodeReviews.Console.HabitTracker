using System.Globalization;

namespace HabitTracker.dewsev;

public static class DateFormatter
{
    public const string DateFormat = "dd/MM/yyyy";
    public static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    
    public static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString(DateFormat, Culture);
    }
}