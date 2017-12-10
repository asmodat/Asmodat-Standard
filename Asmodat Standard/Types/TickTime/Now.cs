using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;
using AsmodatStandard.Extensions;

namespace AsmodatStandard.Types
{
    public static class TickTimeStopwatchSingletonEx
    {
        public static void Restart()
        {
            TickTimeStopwatchSingleton.Instance.TickTimeStopwatch.Restart();
        }

        public static long GetElapsedTicks()
        {
            return TickTimeStopwatchSingleton.Instance.TickTimeStopwatch.ElapsedTicks;
        }

        public static long GetTicks()
        {
            return TickTimeStopwatchSingleton.Instance.TickTimeStopwatch.Ticks;
        }

        public static long GetValue()
        {
            return TickTimeStopwatchSingleton.Instance.TickTimeStopwatch.Value;
        }

    }

    public sealed class TickTimeStopwatchSingleton
    {
        private TickTimeStopwatchSingleton()
        {
            TickTimeStopwatch = new TickTimeStopwatch();
        }

        public TickTimeStopwatch TickTimeStopwatch = null;

        static readonly TickTimeStopwatchSingleton _instance = new TickTimeStopwatchSingleton();
        public static TickTimeStopwatchSingleton Instance
        {
            get { return _instance; }
        }
    }

    public sealed class TickTimeStopwatch
    {
        public TickTimeStopwatch()
        {
            _LastTimeStamp = DateTime.UtcNow.Ticks;
            _Stopwatch = new Stopwatch();
            _Stopwatch.Start();
        }
        
        private long _LastTimeStamp = DateTime.UtcNow.Ticks;
        private Stopwatch _Stopwatch = new Stopwatch();
        private long _LastElapsedTicks = 0;

        public void Restart()
        {
            _LastTimeStamp = DateTime.UtcNow.Ticks;
            _Stopwatch.Restart();
        }

        /// <summary>
        /// start + span
        /// </summary>
        public long Value
        {
            get
            {
                return Ticks + ElapsedTicks;
            }
        }

        public long Ticks
        {
            get
            {
                return _LastTimeStamp;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                long result = _Stopwatch.ElapsedTicks;

                if (result == _LastElapsedTicks)
                    ++result;

                _LastElapsedTicks = result;
                return result;
            }
        }
    }

    //TODO: IFormattable,
    /// <summary>
    /// This class stores and manages DateTime as UTC tick long value, it is safe and efficient to use with serialization instead of DateTime
    /// </summary>
    public partial struct TickTime : IComparable, IEquatable<TickTime>, IEquatable<long>, IComparable<TickTime>, IComparable<long>
    {
        [IgnoreDataMember]
        [XmlIgnore]
        private static long _LastTimeStamp = DateTime.UtcNow.Ticks;

        /// <summary>
        /// Tick time now provides current atomic precition UTC time
        /// </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public static TickTime Now
        {
            get
            {
                long orig, newval = 0, now;

                do
                {
                    orig = _LastTimeStamp;
                    now = DateTime.UtcNow.Ticks;
                    newval = LongEx.Max(new long[]{ now, TickTimeStopwatchSingletonEx.GetValue(), orig + 1});
                } while (Interlocked.CompareExchange(ref _LastTimeStamp, newval, orig) != orig);

                if (now > TickTimeStopwatchSingletonEx.GetTicks())
                    TickTimeStopwatchSingletonEx.Restart();

                return new TickTime(newval);
            }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public static long NowTicks {  get => Now.Ticks; }

        /// <summary>
        /// Set ticts to TickTime.Now
        /// </summary>
        public void SetNow() => Ticks = Now.Ticks;

        public void SetDefault() => Ticks = Default.Ticks;

        public void SetMin() => Ticks = MinValue.Ticks;

        public void SetMax() => Ticks = MaxValue.Ticks;
    }
}
