using System;
using System.Xml;
using AsmodatStandard.Extensions;

namespace AsmodatStandard.Types
{
    public partial struct TickTime : IComparable, IEquatable<TickTime>, IEquatable<long>, IComparable<TickTime>, IComparable<long>
    {
        public static readonly System.DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        public static TickTime FromUnixTimeStamp(double timestamp)
        {
            if (timestamp <= 0)
                return TickTime.Default;

            System.DateTime date = UnixEpoch.AddSeconds(timestamp).ToLocalTime();
            return new TickTime(date);
        }

        public static TickTime FromUnixTimeStamp_ms(double timestamp)
        {
            if (timestamp <= 0)
                return TickTime.Default;

            System.DateTime date = UnixEpoch.AddMilliseconds(timestamp).ToLocalTime();
            return new TickTime(date);
        }


        public static TickTime FromJavaTimeStamp(double timestamp)
        {
            if (timestamp <= 0)
                return TickTime.Default;

            System.DateTime date = UnixEpoch.AddSeconds(Math.Round(timestamp / 1000)).ToLocalTime();
            return new TickTime(date);
        }

        public static double ToUnixTimeStamp(DateTime date) => (date.ToUniversalTime() - UnixEpoch.ToUniversalTime()).TotalSeconds;

        public double ToUnixTimeStamp() => (this.UTC.ToUniversalTime() - UnixEpoch.ToUniversalTime()).TotalSeconds;

        public string ToRFC3339() => XmlConvert.ToString((DateTime)this, XmlDateTimeSerializationMode.Utc);

        public static TickTime FromRFC3339(string date)
        {
            if (date.IsNullOrWhitespace())
                return TickTime.Default;

            return new TickTime(XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Utc));
        }
        
        public string ToString(string format) => ((DateTime)(this)).ToString(format);
    }
}