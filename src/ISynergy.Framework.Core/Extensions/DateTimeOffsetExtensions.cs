﻿using ISynergy.Framework.Core.Constants;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class DateTimeOffsetExtensions.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Converts to universaltimestring.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string ToUniversalTimeString(this DateTimeOffset self) =>
            self.ToUniversalTime().ToString(GenericConstants.DateTimeOffsetFormat);

        /// <summary>
        /// Determines whether [is in range of date] [the specified comparer].
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns><c>true</c> if [is in range of date] [the specified comparer]; otherwise, <c>false</c>.</returns>
        public static bool IsInRangeOfDate(this DateTimeOffset self, DateTimeOffset comparer)
        {
            var start = comparer.ToOffset(self.Offset);
            var end = start.AddDays(1).AddTicks(-1);

            if (self.CompareTo(start) >= 0 && self.CompareTo(end) <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is in range of date] [the specified comparer start].
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="comparer_start">The comparer start.</param>
        /// <param name="comparer_end">The comparer end.</param>
        /// <returns><c>true</c> if [is in range of date] [the specified comparer start]; otherwise, <c>false</c>.</returns>
        public static bool IsInRangeOfDate(this DateTimeOffset self, DateTimeOffset comparer_start, DateTimeOffset comparer_end)
        {
            var start = comparer_start.ToOffset(self.Offset);
            var end = comparer_end.ToOffset(self.Offset);

            if (self.CompareTo(start) >= 0 && self.CompareTo(end) <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compareds to date time.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>System.Int32.</returns>
        public static int ComparedToDateTime(this DateTimeOffset self, DateTimeOffset comparer)
        {
            var source = self.ToOffset(comparer.Offset);
            var target = comparer;

            var result = DateTimeOffset.Compare(source, target);

            return result;
        }

        /// <summary>
        /// Converts to startofday.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToStartOfDay(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, 0, self.Offset);
        }

        /// <summary>
        /// Converts to endofday.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToEndOfDay(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, 0, self.Offset).AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofmonth.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToStartOfMonth(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MinValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 0, 0, 0, 0, self.Offset);
        }

        /// <summary>
        /// Converts to endofmonth.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToEndOfMonth(this DateTimeOffset self)
        {
            if (self == DateTimeOffset.MaxValue)
            {
                return self;
            }

            return new DateTimeOffset(self.Year, self.Month, 1, 0, 0, 0, 0, self.Offset).AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="startOfWeek">The start of week.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToStartOfWeek(this DateTimeOffset self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = (7 + (self.DayOfWeek - startOfWeek)) % 7;
            return self.AddDays(-1 * diff).ToStartOfDay();
        }

        /// <summary>
        /// Converts to endofweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="startOfWeek">The start of week.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToEndOfWeek(this DateTimeOffset self, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return self.ToStartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
        }

        /// <summary>
        /// Converts to startofworkweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToStartOfWorkWeek(this DateTimeOffset self)
        {
            var diff = (7 + (self.DayOfWeek - DayOfWeek.Monday)) % 7;
            return self.AddDays(-1 * diff).ToStartOfDay();
        }

        /// <summary>
        /// Converts to endofworkweek.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset ToEndOfWorkWeek(this DateTimeOffset self)
        {
            return self.ToStartOfWeek(DayOfWeek.Monday).AddDays(5).AddTicks(-1);
        }

        /// <summary>
        /// Gets the possible time zones.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns>System.String.</returns>
        public static string GetPossibleTimeZones(this DateTimeOffset self, string seperator = ", ")
        {
            var offset = self.Offset;
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var result = new List<string>();

            foreach (var timeZone in timeZones)
            {
                if (timeZone.GetUtcOffset(self.DateTime).Equals(offset))
                    result.Add(timeZone.DisplayName);
            }

            return string.Join(seperator, result);
        }
    }
}
