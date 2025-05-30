﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LANFile.Converters;

public class SvgFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s) return $"/Assets/Svg/{s}.svg";

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}