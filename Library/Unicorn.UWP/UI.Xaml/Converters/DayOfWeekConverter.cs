// Copyright (c) 2016 Joe Wen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE

using System;
using Windows.UI.Xaml.Data;

namespace Unicorn
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = (string)parameter;
            format = string.IsNullOrEmpty(format) ? string.Empty : $"_{format}";

            var weekday = (DayOfWeek)value;
            switch (weekday)
            {
                case DayOfWeek.Sunday:
                    return ResourceManager.GetString($"STRING_ID_SUNDAY{format}");
                case DayOfWeek.Monday:
                    return ResourceManager.GetString($"STRING_ID_MONDAY{format}");
                case DayOfWeek.Tuesday:
                    return ResourceManager.GetString($"STRING_ID_TUESDAY{format}");
                case DayOfWeek.Wednesday:
                    return ResourceManager.GetString($"STRING_ID_WEDNESDAY{format}");
                case DayOfWeek.Thursday:
                    return ResourceManager.GetString($"STRING_ID_THURSDAY{format}");
                case DayOfWeek.Friday:
                    return ResourceManager.GetString($"STRING_ID_FRIDAY{format}");
                case DayOfWeek.Saturday:
                    return ResourceManager.GetString($"STRING_ID_SATURDAY{format}");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
