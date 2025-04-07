using AutoMapper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace compete_platform.Infrastructure.Services.AggregationFunctions
{
   
    public static class DateTimeToRuFormatStringConverters
    {
        public static string Convert(DateTime source)
        {
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            CultureInfo culture = new CultureInfo("ru-RU");
            var date = source;
            DateTime moscowDateTime = TimeZoneInfo.ConvertTimeFromUtc(date, moscowTimeZone);
            return moscowDateTime.ToString("d MMMM, yyyy", culture);
        }
    }
    
}
