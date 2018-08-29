using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Threading;
using System.Diagnostics;
using System;
using AsmodatStandard.Types;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandardTest.Threading.CronTest
{
    [TestFixture]
    public class CronTest
    {
        [Test]
        public void RangeCronTest()
        {
            void Test(string cron, DateTime d, bool equal)
            {
                var c = CronEx.ToCron(cron);
                if(equal)
                    Assert.AreEqual(0, c.Compare(d));
                else
                    Assert.AreNotEqual(0, c.Compare(d));
            }

            Test("5-5 * * * * *",
                new DateTime(1, 1, 1, 1, 5, 0, DateTimeKind.Utc),
                true);

            Test("* 6-6 * * * *",
                new DateTime(1, 1, 1, 5, 5, 0, DateTimeKind.Utc),
                false);

            Test("50-10 * * * * *",
                new DateTime(1, 1, 1, 1, 5, 0, DateTimeKind.Utc),
                true);

            Test("50-10 * * * * *",
                new DateTime(1, 1, 1, 1, 15, 0, DateTimeKind.Utc),
                false);
        }

        [Test]
        public void RandomCompareCronTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var dtStart = RandomEx.DateTime().Truncate(TimeSpan.FromMinutes(1));
                var c = CronEx.ToCron($"{(RandomEx.NextBool() ? dtStart.Minute.ToString() : "*")} {(RandomEx.NextBool() ? dtStart.Hour.ToString() : "*")} {(RandomEx.NextBool() ? dtStart.Day.ToString() : "*")} {(RandomEx.NextBool() ? dtStart.Month.ToString() : "*")} {(RandomEx.NextBool() ? ((int)dtStart.DayOfWeek + 1).ToString() : "*")} {(RandomEx.NextBool() ? dtStart.Year.ToString() : "*")}");

                for (int i2 = 0; i2 < 1000; i2++)
                {
                    DateTime dt = default(DateTime);
                    bool success = false;
                    var sw = Stopwatch.StartNew();
                    do
                    {
                        dt = RandomEx.DateTime().Truncate(TimeSpan.FromMinutes(1));

                        var m = c.Minutes.AllValues ? dtStart.Minute : dt.Minute;
                        var h = c.Hours.AllValues ? dtStart.Hour : dt.Hour;
                        var d = c.DayOfMonth.AllValues ? dtStart.Day : dt.Day;
                        var M = c.Month.AllValues ? dtStart.Month : dt.Month;
                        var y = c.Year.AllValues ? dtStart.Year : dt.Year;

                        try
                        {
                            dt = new DateTime(y, M, d, h, m, 0, DateTimeKind.Utc);
                            var ticks = dt.Ticks;
                            success = true;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            continue;
                        }
                    } while (!success || sw.ElapsedMilliseconds > 60000);

                    if (!success) //failed generation
                        continue;

                    var cmp = c.Compare(dt);

                    if (dt.Ticks == dtStart.Ticks)
                        Assert.AreEqual(0, cmp);
                    if (dtStart.Ticks > dt.Ticks)
                        Assert.Greater(cmp, 0);
                    else if (dtStart.Ticks < dt.Ticks)
                        Assert.Less(cmp, 0);
                }
            }
        }

        [Test]
        public void RandomMultivalCronTest()
        {
            for (int i = 0; i < 100; i++)
            {
                int[] m_arr = RandomEx.Next(0, 60, RandomEx.Next(1, 60)).Distinct().ToArray();
                int[] h_arr = RandomEx.Next(0, 24, RandomEx.Next(1, 24)).Distinct().ToArray();
                int[] d_arr = RandomEx.Next(1, 32, RandomEx.Next(1, 32)).Distinct().ToArray();
                int[] M_arr = RandomEx.Next(1, 13, RandomEx.Next(1, 13)).Distinct().ToArray();
                int[] D_arr = RandomEx.Next(1, 8, RandomEx.Next(1, 8)).Distinct().ToArray();
                int[] y_arr = RandomEx.Next(0, 9999, RandomEx.Next(1, 1000)).Distinct().ToArray();

                var mVal = RandomEx.NextBool() ? "*" : m_arr.StringJoin();
                var hVal = RandomEx.NextBool() ? "*" : h_arr.StringJoin();
                var dVal = RandomEx.NextBool() ? "*" : d_arr.StringJoin();
                var MVal = RandomEx.NextBool() ? "*" : M_arr.StringJoin();
                var DVal = RandomEx.NextBool() ? "*" : D_arr.StringJoin();
                var yVal = RandomEx.NextBool() ? "*" : y_arr.StringJoin();

                var c = CronEx.ToCron($"{mVal} {hVal} {dVal} {MVal} {DVal} {yVal}");

                for (int i2 = 0; i2 < 1000; i2++)
                {
                    var dt = RandomEx.DateTime().Truncate(TimeSpan.FromMinutes(1));
                    var cmp = c.Compare(dt);

                    var mPass = mVal == "*";
                    var hPass = hVal == "*";
                    var dPass = dVal == "*";
                    var MPass = MVal == "*";
                    var DPass = DVal == "*";
                    var yPass = yVal == "*";

                    if (cmp == 0)
                    {
                        if (!mPass)
                            Assert.IsTrue(m_arr.Any(x => x == dt.Minute));

                        if (!hPass)
                            Assert.IsTrue(h_arr.Any(x => x == dt.Hour));

                        if (!dPass)
                            Assert.IsTrue(d_arr.Any(x => x == dt.Day));

                        if (!MPass)
                            Assert.IsTrue(M_arr.Any(x => x == dt.Month));

                        if (!DPass)
                            Assert.IsTrue(D_arr.Any(x => x == ((int)dt.DayOfWeek + 1)));

                        if (!yPass)
                            Assert.IsTrue(y_arr.Any(x => x == dt.Year));
                    }
                    else
                    {
                        bool fail = false;

                        if (!mPass)
                            fail = (m_arr.All(x => x != dt.Minute));

                        if (!fail && !hPass)
                            fail = (h_arr.All(x => x != dt.Hour));

                        if (!fail && !dPass)
                            fail = (d_arr.All(x => x != dt.Day));

                        if (!fail && !MPass)
                            fail = (M_arr.All(x => x != dt.Month));

                        if (!fail && !DPass)
                            fail = (D_arr.All(x => x != ((int)dt.DayOfWeek + 1)));

                        if (!fail && !yPass)
                            fail = (y_arr.All(x => x != dt.Year));

                        Assert.IsTrue(fail);
                    }
                }
            }
        }

        [Test]
        public void RandomMultivalRangeCronTest()
        {
            bool VerifyEq(bool pass, int[] arr, int dtVal)
            {
                if (pass)
                    return true;

                for (int i = 0; i < arr.Length; i += 2)
                {
                    var min = arr[i];
                    var max = arr[i + 1];

                    if (min <= max)
                    {
                        if (dtVal >= min && dtVal <= max)
                            return true;
                    }
                    else if (min >= max)
                    {
                        if (dtVal >= min || dtVal <= max)
                            return true;
                    }
                }

                return false;
            }

            for (int i = 0; i < 1000; i++)
            {
                int[] m_arr = RandomEx.Next(0, 60, RandomEx.NextEven(1, 17)).Distinct().ToEvenArray();
                int[] h_arr = RandomEx.Next(0, 24, RandomEx.NextEven(1, 7)).Distinct().ToEvenArray();
                int[] d_arr = RandomEx.Next(1, 32, RandomEx.NextEven(1, 9)).Distinct().ToEvenArray();
                int[] M_arr = RandomEx.Next(1, 13, RandomEx.NextEven(1, 5)).Distinct().ToEvenArray();
                int[] D_arr = RandomEx.Next(1, 8, RandomEx.NextEven(1, 4)).Distinct().ToEvenArray();
                int[] y_arr = RandomEx.Next(0, 9999, RandomEx.NextEven(2, 100)).Distinct().ToEvenArray();

                var mVal = m_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : m_arr.StringPairJoin();
                var hVal = h_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : h_arr.StringPairJoin();
                var dVal = d_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : d_arr.StringPairJoin();
                var MVal = M_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : M_arr.StringPairJoin();
                var DVal = D_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : D_arr.StringPairJoin();
                var yVal = y_arr.IsNullOrEmpty() || RandomEx.NextBool() ? "*" : y_arr.StringPairJoin();

                var c = CronEx.ToCron($"{mVal} {hVal} {dVal} {MVal} {DVal} {yVal}");

                for (int i2 = 0; i2 < 1000; i2++)
                {
                    var dt = RandomEx.DateTime().Truncate(TimeSpan.FromMinutes(1));
                    var cmp = c.Compare(dt);

                    var mPass = mVal == "*";
                    var hPass = hVal == "*";
                    var dPass = dVal == "*";
                    var MPass = MVal == "*";
                    var DPass = DVal == "*";
                    var yPass = yVal == "*";

                    if (cmp == 0)
                    {
                        Assert.IsTrue(VerifyEq(mPass, m_arr, dt.Minute));
                        Assert.IsTrue(VerifyEq(hPass, h_arr, dt.Hour));
                        Assert.IsTrue(VerifyEq(dPass, d_arr, dt.Day));
                        Assert.IsTrue(VerifyEq(MPass, M_arr, dt.Month));
                        Assert.IsTrue(VerifyEq(DPass, D_arr, ((int)dt.DayOfWeek + 1)));
                        Assert.IsTrue(VerifyEq(yPass, y_arr, dt.Year));
                    }
                    else
                    {
                        Assert.IsFalse(
                            VerifyEq(mPass, m_arr, dt.Minute) &&
                            VerifyEq(hPass, h_arr, dt.Hour) &&
                            VerifyEq(dPass, d_arr, dt.Day) &&
                            VerifyEq(MPass, M_arr, dt.Month) &&
                            VerifyEq(DPass, D_arr, ((int)dt.DayOfWeek + 1)) &&
                            VerifyEq(yPass, y_arr, dt.Year));
                    }
                }
            }
        }

        [Test]
        public void RandomModuloCronTest()
        {
            for (int i = 0; i < 100; i++)
            {
                int m_mod = RandomEx.Next(1, 60);
                int h_mod = RandomEx.Next(1, 24);
                int d_mod = RandomEx.Next(1, 32);
                int M_mod = RandomEx.Next(1, 13);
                int D_mod = RandomEx.Next(1, 8);
                int y_mod = RandomEx.Next(1, 100);

                int m_shift = RandomEx.Next(1, 60);
                int h_shift = RandomEx.Next(1, 24);
                int d_shift = RandomEx.Next(1, 32);
                int M_shift = RandomEx.Next(1, 13);
                int D_shift = RandomEx.Next(1, 8);
                int y_shift = RandomEx.Next(1, 100);

                var mVal = RandomEx.NextBool() ? "*" : $"{m_shift}/{m_mod}";
                var hVal = RandomEx.NextBool() ? "*" : $"{h_shift}/{h_mod}";
                var dVal = RandomEx.NextBool() ? "*" : $"{d_shift}/{d_mod}";
                var MVal = RandomEx.NextBool() ? "*" : $"{M_shift}/{M_mod}";
                var DVal = RandomEx.NextBool() ? "*" : $"{D_shift}/{D_mod}";
                var yVal = RandomEx.NextBool() ? "*" : $"{y_shift}/{y_mod}";

                var c = CronEx.ToCron($"{mVal} {hVal} {dVal} {MVal} {DVal} {yVal}");

                for (int i2 = 0; i2 < 1000; i2++)
                {
                    var dt = RandomEx.DateTime().Truncate(TimeSpan.FromMinutes(1));
                    var cmp = c.Compare(dt);

                    var mPass = mVal == "*";
                    var hPass = hVal == "*";
                    var dPass = dVal == "*";
                    var MPass = MVal == "*";
                    var DPass = DVal == "*";
                    var yPass = yVal == "*";

                    if (cmp == 0)
                    {
                        if (!mPass)
                            Assert.IsTrue((dt.Minute - m_shift) % m_mod == 0);

                        if (!hPass)
                            Assert.IsTrue((dt.Hour - h_shift) % h_mod == 0);

                        if (!dPass)
                            Assert.IsTrue((dt.Day - d_shift) % d_mod == 0);

                        if (!MPass)
                            Assert.IsTrue((dt.Month - M_shift) % M_mod == 0);

                        if (!DPass)
                            Assert.IsTrue((((int)dt.DayOfWeek + 1) - D_shift) % D_mod == 0);

                        if (!yPass)
                            Assert.IsTrue((dt.Year - y_shift) % y_mod == 0);
                    }
                    else
                    {
                        bool fail = false;

                        if (!mPass)
                            fail = ((dt.Minute - m_shift) % m_mod != 0);

                        if (!fail && !hPass)
                            fail = ((dt.Hour - h_shift) % h_mod != 0);

                        if (!fail && !dPass)
                            fail = ((dt.Day - d_shift) != 0);

                        if (!fail && !MPass)
                            fail = ((dt.Month - M_shift) != 0);

                        if (!fail && !DPass)
                            fail = ((((int)dt.DayOfWeek + 1) - D_shift) % D_mod != 0);

                        if (!fail && !yPass)
                            fail = ((dt.Year - y_shift) % y_mod != 0);

                        Assert.IsTrue(fail);
                    }
                }
            }
        }
    }
}
