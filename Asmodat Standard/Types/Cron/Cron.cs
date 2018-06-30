namespace AsmodatStandard.Types
{
    /// <summary>
    /// https://docs.aws.amazon.com/lambda/latest/dg/tutorial-scheduled-events-schedule-expressions.html
    /// </summary>
    public class Cron
    {
        /// <summary>
        /// Optional paramter for debug purpouses that defines origin of the cron if it was created via parsing the string
        /// </summary>
        public string Origin { get; private set; }

        public CronValue Minutes { get; private set; }
        public CronValue Hours { get; private set; }
        public CronValue DayOfMonth { get; private set; }
        public CronValue Month { get; private set; }
        public CronValue DayOfWeek { get; private set; }
        public CronValue Year { get; private set; }

        public Cron(CronValue minutes, CronValue hours, CronValue dayOfMonth, CronValue month, CronValue dayOfWeek, CronValue year, string origin = null)
        {
            Minutes = minutes;
            Hours = hours;
            DayOfMonth = dayOfMonth;
            Month = month;
            DayOfWeek = dayOfWeek;
            Year = year;
            Origin = origin;
        }
    }
}
