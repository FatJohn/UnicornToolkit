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
    public class BooleanToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return new Thickness(0);
            }
            else
            {
                bool boolValue = (bool)value;

                Thickness targetThickness = ParseThickness(parameter);

                return boolValue ? targetThickness : new Thickness(0);
            }
        }

        private Thickness ParseThickness(object parameter)
        {
            Thickness result = new Thickness(0);

            string targetThicknessString = parameter as string;
            if (string.IsNullOrWhiteSpace(targetThicknessString) == false && targetThicknessString.Contains(","))
            {
                var stringArray = targetThicknessString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (stringArray.Length == 4)
                {
                    List<int> targetThicknessValues = new List<int>();
                    int value = -1;
                    foreach (var s in stringArray)
                    {
                        if (int.TryParse(s, out value))
                        {
                            targetThicknessValues.Add(value);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (targetThicknessValues.Count == 4)
                    {
                        result = new Thickness(targetThicknessValues[0], targetThicknessValues[1], targetThicknessValues[2], targetThicknessValues[3]);
                    }
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
