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

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Unicorn.UI.Xaml.Controls
{
    [TemplatePart(Name = RootBorderName, Type = typeof(Border))]
    public sealed class WaitDialog : Control
    {
        private const string RootBorderName = "RootBorder";

        private Popup rootPopup;
        private Border rootBorder;

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(WaitDialog), new PropertyMetadata(null));

        public WaitDialog()
        {
            this.DefaultStyleKey = typeof(WaitDialog);

            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            rootPopup = new Popup()
            {
                IsLightDismissEnabled = false,
                IsOpen = false,
                Child = this,
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rootBorder = (Border)GetTemplateChild(RootBorderName);

            ResizeContainers();

            Window.Current.SizeChanged += OnWindowSizeChanged;
            Unloaded += OnUnloaded;
        }

        private void ResizeContainers()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (rootBorder != null)
            {
                rootBorder.TrySetWidth(Window.Current.Bounds.Width);
                rootBorder.TrySetHeight(Window.Current.Bounds.Height);
            }
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            ResizeContainers();
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Unloaded -= OnUnloaded;
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        public void Show(string message)
        {
            Message = message;

            if (!rootPopup.IsOpen)
            {
                rootPopup.IsOpen = true;
            }
        }

        public void Close()
        {
            rootPopup.IsOpen = false;
        }
    }
}
