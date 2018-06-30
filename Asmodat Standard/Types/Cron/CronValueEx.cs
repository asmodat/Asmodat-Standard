using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AsmodatStandard.Extensions;
using static AsmodatStandard.Types.CronValue;

namespace AsmodatStandard.Types
{
    public static class CronValueEx
    {
        public static int Min(this CronUnit cu)
        {
            switch (cu)
            {
                case CronUnit.Minutes: return 0;
                case CronUnit.Hours: return 0;
                case CronUnit.DayOfMonth: return 1;
                case CronUnit.DayOfWeek: return 1;
                case CronUnit.Month: return 1;
                case CronUnit.Year: return int.MinValue;
                default: throw new NotSupportedException();
            }
        }

        public static int Max(this CronUnit cu)
        {
            switch (cu)
            {
                case CronUnit.Minutes: return 59;
                case CronUnit.Hours: return 23;
                case CronUnit.DayOfMonth: return 31;
                case CronUnit.DayOfWeek: return 7;
                case CronUnit.Month: return 12;
                case CronUnit.Year: return int.MaxValue;
                default: throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Months and abbreviations
        /// </summary>
        public static readonly Dictionary<string, int> Months = new Dictionary<string, int>()
        {
            { "JAN", 1 },  { "JANUARY", 1 },
            { "FEB", 2 },  { "FEBRUARY", 2 },
            { "MAR", 3 },  { "MARCH", 3 },
            { "APR", 4 },  { "APRIL", 4 },
            { "MAY", 5 },  { "MAY", 5 },
            { "JUN", 6 },  { "JUNE", 6 },
            { "JUL", 7 },  { "JULY", 7 },
            { "AUG", 8 },  { "AUGUST", 8 },
            { "SEP", 9 }, { "SEPT", 9 }, { "SEPTEMBER", 9 },
            { "OCT", 10 },  { "OCOBER", 10 },
            { "NOV", 11 },  { "NOVEMBER", 11 },
            { "DEC", 12 },  { "DECEMBER", 12 },
        };

        /// <summary>
        /// Days and abbreviations
        /// </summary>
        public static readonly Dictionary<string, int> Days = new Dictionary<string, int> ()
        {
            { "SUN", 1 },
            { "MON", 2 },
            { "TU", 3 }, { "TUE", 3 }, { "TUES", 3 }, { "TUESDAY", 3 },
            { "WED", 4 }, { "WEDNESDAY", 4 },
            { "TH", 5 }, { "THU", 5 }, { "THUR", 5 }, { "THURS", 5 }, { "THURSDAY", 5 },
            { "FRI", 6 }, { "FRIDAY", 6 },
            { "SAT", 7 }, { "SATURDAY", 7 }
        };

        public static CronValue ToCronValue(string cronValueExpression, CronUnit unit)
        {
            var exp = cronValueExpression?.ToLower().Trim();
            if (exp.IsNullOrEmpty())
                throw new ArgumentException($"Expression can't be null or empty after the trim. Oryginal value '{cronValueExpression ?? "null"}'.");

            if (exp == "?" && (unit == CronUnit.DayOfMonth || unit == CronUnit.DayOfWeek))
                return new CronValue(unit, unspecified: true, allValues: false, last: false, first: false);

            if (exp == "*")
                return new CronValue(unit, unspecified: false, allValues: true, last: false, first: false);

            if (exp.Contains(","))
            {
                var innerExpressions = exp.Split(',').Select(ex => ToCronInnerValue(ex, unit)).ToArray();
                if (innerExpressions.Length <= 0)
                    throw new Exception($"Failed to eveluate '{cronValueExpression}' with unit: '{unit}', could not find any inner expressions even though ',' separator was present.");

                return new CronValue(unit, innerExpressions);
            }

            return new CronValue(unit, new CronValue[1] { ToCronInnerValue(exp, unit) });
        }

        public static CronValue ToCronInnerValue(string innerExpression, CronUnit type)
        {
            var exp = innerExpression?.ToLower().Trim();
            if (exp.IsNullOrEmpty())
                throw new ArgumentException($"Inner expression can't be null or empty after the trim. Original value: '{innerExpression ?? "null"}', Unit: '{type}'.");

            if (type != CronUnit.Year)
            {
                if (exp.Equals("F", comparisonType: StringComparison.InvariantCultureIgnoreCase)) //first
                    return new CronValue(type, unspecified: false, allValues: false, last: false, first: true);
                else if (exp.Equals("L", comparisonType: StringComparison.InvariantCultureIgnoreCase)) //last
                    return new CronValue(type, unspecified: false, allValues: false, last: true, first: false);
            }

            if (int.TryParse(exp, out var numb))
                return new CronValue(type, rangeStart: numb, rangeEnd: numb, n: -1);

            if (exp.Contains('-'))
            {
                var split = exp.Split('-');

                if (split.Length != 2)
                    throw new Exception($"Inner expression has range separator '-' with {split.Length} instead of 2 arguments. Expression:'{innerExpression}', Unit: '{type}'.");

                var rStart = ParseCronValueToNumber(split[0], type);
                var rEnd = ParseCronValueToNumber(split[1], type);
                return new CronValue(type, rangeStart: rStart, rangeEnd: rEnd, n: -1);
            }

            if (exp.Contains('#') && type == CronUnit.DayOfWeek) //defines n'th day of month
            {
                var split = exp.Split('#');

                if (split.Length != 2)
                    throw new Exception($"Inner expression has nd/nth separator '#' with {split.Length} instead of 2 arguments. Expression:'{innerExpression}', Unit: '{type}'.");

                var dayOfWeek = ParseCronValueToNumber(split[0], CronUnit.DayOfWeek);
                var n = split[1].ToIntOrDefault(-1);

                if (n < 0 || n > 4)
                    throw new Exception($"Inner expression has nd/nth separator with value '{n}', its out of allower range <1; 4>. Expression:'{innerExpression}', Unit: '{type}'.");

                return new CronValue(type, rangeStart: dayOfWeek, rangeEnd: dayOfWeek, n: n);
            }

            if (exp.Contains('/'))
            {
                var split = exp.Split('/');

                if (split.Length != 2)
                    throw new Exception($"Inner expression has increment separator '/' with {split.Length} instead of 2 arguments. Expression:'{innerExpression}', Unit: '{type}'.");

                if(split[1].Equals("W", StringComparison.InvariantCultureIgnoreCase))
                {
                    var day = ParseCronValueToNumber(split[0], type);
                    return new CronValue(type, rangeStart: day, rangeEnd: day, n: -1, weekday: true);
                }

                var shift = split[0] == "0" ? 0 : ParseCronValueToNumber(split[0], type);
                var modulo = ParseCronValueToNumber(split[1], type);

                return new CronValue(type, incrementShift: shift, incrementModulo: modulo);
            }

            throw new NotSupportedException($"Could not evaluate inner expression '{innerExpression}', Unit: '{type}'");
        }

        private static int ParseCronValueToNumber(string numb, CronUnit type)
        {
            var val = numb.ToIntOrDefault(-1);

            if(val < 0 && type == CronUnit.Month)
            {
                var month = Months.Keys.FirstOrDefault(x => x.Equals(numb.Trim(' ', '.'), StringComparison.InvariantCultureIgnoreCase));
                if (month != null)
                    val = Months[month];
            }
            else if(val < 0 && type == CronUnit.DayOfWeek)
            {
                var day = Days.Keys.FirstOrDefault(x => x.Equals(numb.Trim(' ', '.'), StringComparison.InvariantCultureIgnoreCase));
                if (day != null)
                    val = Days[day];
            }

            if (val < type.Min() || val > type.Max())
                throw new ArgumentException($"Invalid value '{val}' for '{type}' unit.");

            return val;
        }
    }
}
