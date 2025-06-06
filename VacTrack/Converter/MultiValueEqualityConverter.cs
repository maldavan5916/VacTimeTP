﻿using System.Globalization;
using System.Windows.Data;

namespace VacTrack.Converter
{
    public sealed class MultiValueEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values?.All(o => o?.Equals(values[0]) == true) == true || values?.All(o => o == null) == true;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
