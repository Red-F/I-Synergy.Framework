﻿using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public static class DateTimeOffsetConverter
    {
        /// <summary>
        /// Converts from DateTime to TimaSpan.
        /// </summary>
        /// <param name="dt">The source DateTime value.</param>
        /// <returns>Returns the time portion of DateTime in the form of TimeSpan if succeeded, null otherwise.</returns>
        public static TimeSpan? DateTimeOffsetToTimeSpan(DateTimeOffset dt)
        {
            if (dt == DateTimeOffset.MinValue || dt == DateTimeOffset.MaxValue)
            {
                return new TimeSpan(0);
            }
            else
            {
                return dt - dt.Date;
            }
        }

        /// <summary>
        /// Converts from Timespan to DateTime.
        /// </summary>
        /// <param name="ts">The source TimeSpan value.</param>
        /// <returns>Returns a DateTime filled with date equals to mindate and time equals to time in timespan if succeeded, null otherwise.</returns>
        public static DateTimeOffset? TimeSpanToDateTimeOffset(DateTimeOffset dt, TimeSpan ts)
        {
            return new DateTimeOffset(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, dt.Offset);
        }
    }

    public class DateTimeOffsetToTimeSpanConverter : IValueConverter
    {
        private DateTimeOffset original;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset dt)
            {
                original = dt;
                var ts = DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(dt);
                return ts.GetValueOrDefault(TimeSpan.MinValue);
            }

            return TimeSpan.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(original is DateTimeOffset odt && value is TimeSpan ts)
            {
                var dt = DateTimeOffsetConverter.TimeSpanToDateTimeOffset(odt, ts);
                return dt.GetValueOrDefault(DateTimeOffset.MinValue);
            }

            return DateTimeOffset.MinValue;
        }
    }

    public class DateOffsetCollectionToDateTimeCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = new ObservableCollection<DateTime>();

            if (value != null && value.GetType() == typeof(ObservableCollection<DateTimeOffset>))
            {
                var collection = value as ObservableCollection<DateTimeOffset>;

                foreach (var item in collection)
                {
                    result.Add(item.ToLocalTime().DateTime);
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var result = new ObservableCollection<DateTimeOffset>();

            if (value != null)
            {
                if (value.GetType() == typeof(ObservableCollection<DateTime>))
                {
                    var collection = value as ObservableCollection<DateTime>;

                    foreach (var item in collection)
                    {
                        result.Add(new DateTimeOffset(DateTime.SpecifyKind(item, DateTimeKind.Local)));
                    }
                }

                if (value.GetType() == typeof(ObservableCollection<string>))
                {
                    var collection = value as ObservableCollection<string>;

                    foreach (var item in collection)
                    {
                        result.Add(new DateTimeOffset(DateTime.SpecifyKind(DateTime.Parse(item), DateTimeKind.Local)));
                    }
                }
            }

            return result;
        }
    }

    public class DateTimeOffsetToLocalDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset datetime)
            {
                return datetime.ToLocalTime();
            }

            return DateTimeOffset.Now.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset datetime)
            {
                return datetime.ToUniversalTime();
            }

            return DateTimeOffset.Now.ToUniversalTime();
        }
    }

    public class DateTimeOffsetToLocalDateStringConverter : IValueConverter
    {
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:d\} 2009-06-15T13:45:30 -> 6/15/2009 (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:D\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:f\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:F\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45:30 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:g\} 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:G\} 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=T} 2009-06-15T13:45:30 -> 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=T} 2009-06-15T13:45:30 -> 1:45:30 PM (en-US)

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset datetime)
            {
                if (parameter != null)
                {
                    return datetime.ToLocalTime().ToString(parameter.ToString());
                }

                return datetime.ToLocalTime().ToString("f");
            }

            return DateTimeOffset.Now.ToString("f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
