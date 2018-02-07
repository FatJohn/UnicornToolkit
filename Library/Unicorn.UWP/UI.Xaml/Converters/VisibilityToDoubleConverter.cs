// Copyright (c) 2016 Louis Wu

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
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Unicorn
{
    public class VisibilityToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                Visibility visibilityValue = (Visibility)value;

                var doublePair = ParseDouble(parameter);

                return visibilityValue == Visibility.Visible ? doublePair.Item1 : doublePair.Item2;
            }
        }

        private Tuple<double, double> ParseDouble(object parameter)
        {
            var result = Tuple.Create(0.0, 0.0);

            string targetThicknessString = parameter as string;
            if (string.IsNullOrWhiteSpace(targetThicknessString) == false && targetThicknessString.Contains(","))
            {
                var stringArray = targetThicknessString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (stringArray.Length == 2)
                {
                    double firstDouble = 0;
                    double secondDouble = 0;

                    double.TryParse(stringArray[0], out firstDouble);
                    double.TryParse(stringArray[1], out secondDouble);

                    result = Tuple.Create(firstDouble, secondDouble);
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
