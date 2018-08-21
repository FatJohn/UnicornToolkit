// Copyright (c) 2018 Louis Wu

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

using Windows.UI.Xaml;

namespace Unicorn
{
    public static class FrameworkElementExtensions
    {
        public static bool TrySetWidth(this FrameworkElement element, double targetWidth)
        {
            try
            {
                if (element.Width == targetWidth)
                {
                    return false;
                }

                element.Width = targetWidth;
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool TrySetHeight(this FrameworkElement element, double targetHeight)
        {
            try
            {
                if (element.Height == targetHeight)
                {
                    return false;
                }

                element.Height = targetHeight;
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool TrySetMargin(this FrameworkElement element, Thickness targetMargin)
        {
            try
            {
                if (element.Margin == targetMargin)
                {
                    return false;
                }

                element.Margin = targetMargin;
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
