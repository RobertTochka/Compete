using Compete_POCO_Models.Infrastrcuture.Data;

namespace compete_platform.Infrastructure.ValueResolvers
{
    public class DateIntervals
    {
        public static (DateTime StartDate, DateTime EndDate) GetDateInterval(string interval)
        {
            if (interval == AppDictionary.Month)
                return GetLastMonthInterval();
            if (interval == AppDictionary.Day)
                return GetLastDayInterval();
            if (interval == AppDictionary.Week)
                return GetLastWeekInterval();
            if(interval == AppDictionary.All)
                return GetAllInterval();
            throw new ArgumentNullException();
        }
        public static (DateTime StartDate, DateTime EndDate) GetLastDayInterval()
        {
            DateTime todayUtc = DateTime.UtcNow.Date; 
            DateTime endDateUtc = todayUtc.AddDays(1).AddTicks(-1);
            return (todayUtc, endDateUtc);
        }
        public static (DateTime StartDate, DateTime EndDate) GetAllInterval()
        {
            DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); 
            DateTime endDate = new DateTime(2100, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            return (startDate, endDate);
        }

        public static (DateTime StartDate, DateTime EndDate) GetLastWeekInterval()
        {
            DateTime endDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
            DateTime startDate = endDate.AddDays(-7);
            return (startDate, endDate);
        }

        public static (DateTime StartDate, DateTime EndDate) GetLastMonthInterval()
        {
            DateTime endDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
            DateTime startDate = endDate.AddMonths(-1);
            return (startDate, endDate);
        }
    }
}
