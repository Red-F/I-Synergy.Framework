﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class ZeroDoubleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((double)value != 0 || value != null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleOfDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double result = 0;

            if (double.TryParse(value.ToString(), out var size) && double.TryParse(parameter.ToString(), out var margin))
            {
                var calc = size - margin;
                if (calc >= 0) result = calc;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleIsLesserThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double.TryParse(parameter.ToString(), out var limit);
            double.TryParse(value.ToString(), out var test);

            if (test < limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleIsLesserOrEqualThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double.TryParse(parameter.ToString(), out var limit);
            double.TryParse(value.ToString(), out var test);

            if (test <= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleIsGreaterThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double.TryParse(parameter.ToString(), out var limit);
            double.TryParse(value.ToString(), out var test);

            if (test > limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleIsGreaterOrEqualThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double.TryParse(parameter.ToString(), out var limit);
            double.TryParse(value.ToString(), out var test);

            if (test >= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}