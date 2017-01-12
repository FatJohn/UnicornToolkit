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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Unicorn.UI.Xaml
{
    public static class FlyoutProperty
    {
        #region IsOpenFlyoutProperty
        public static readonly DependencyProperty IsFlyoutOpenProperty =
            DependencyProperty.RegisterAttached("IsFlyoutOpen", typeof(bool), typeof(FlyoutProperty), new PropertyMetadata(false, OnIsFlyoutOpenPropertyChanged));

        public static void SetIsFlyoutOpen(Flyout element, bool value)
        {
            element?.SetValue(IsFlyoutOpenProperty, value);
        }
        public static bool GetIsFlyoutOpen(Flyout element)
        {
            if (element == null)
            {
                return false;
            }

            return (bool)element.GetValue(IsFlyoutOpenProperty);
        }
        #endregion

        #region ParentProperty
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached("Parent", typeof(object), typeof(FlyoutProperty), new PropertyMetadata(false, OnParentPropertyChanged));

        public static void SetParent(Flyout element, object value)
        {
            element?.SetValue(ParentProperty, value);
        }
        public static object GetParent(Flyout element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(ParentProperty);
        }
        #endregion

        #region ParentProperty
        private static void OnParentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as Flyout;

            if (flyout == null)
            {
                return;
            }

            flyout.Opened -= Flyout_Opened;
            flyout.Opened += Flyout_Opened;

            flyout.Closed -= Flyout_Closed;
            flyout.Closed += Flyout_Closed;
        }

        private static void Flyout_Closed(object sender, object e)
        {
            Flyout flyout = sender as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.SetValue(IsFlyoutOpenProperty, false);
        }

        private static void Flyout_Opened(object sender, object e)
        {
            Flyout flyout = sender as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.SetValue(IsFlyoutOpenProperty, true);
        }

        #endregion

        #region IsOpenFlyoutProperty
        private static void OnIsFlyoutOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as Flyout;
            var parent = d.GetValue(ParentProperty) as FrameworkElement;

            if (flyout == null || parent == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                flyout.ShowAt(parent);
            }
            else
            {
                flyout.Hide();
            }
        }
        #endregion
    }
}
