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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Unicorn.UI.Xaml.Controls
{
    /// <summary>
    /// 支援 DataBinding，但不支援初始化為 null。
    /// </summary>
    [TemplatePartAttribute(Name = "ThreeStateImage", Type = typeof(BitmapImage))]
    public class ThreeStateImageButton : Button
    {
        public static readonly DependencyProperty NormalImageUriProperty =
            DependencyProperty.Register("NormalImageUri", typeof(Uri), typeof(ThreeStateImageButton), new PropertyMetadata(null, OnNormalImageUriChanged));

        public Uri NormalImageUri
        {
            get { return (Uri)GetValue(NormalImageUriProperty); }
            set { SetValue(NormalImageUriProperty, value); }
        }

        private static void OnNormalImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThreeStateImageButton sourceControl = d as ThreeStateImageButton;
            if (sourceControl != null &&
                sourceControl.threeStateImage != null &&
                sourceControl.IsEnabled == true &&
                sourceControl.IsPointerOver == false &&
                sourceControl.IsPressed == false)
            {
                sourceControl.threeStateImage.Source = new BitmapImage() { UriSource = sourceControl.NormalImageUri };
            }
        }

        public static readonly DependencyProperty PointerOverImageUriProperty =
            DependencyProperty.Register("PointerOverImageUri", typeof(Uri), typeof(ThreeStateImageButton), new PropertyMetadata(null));

        public Uri PointerOverImageUri
        {
            get { return (Uri)GetValue(PointerOverImageUriProperty); }
            set { SetValue(PointerOverImageUriProperty, value); }
        }

        public static readonly DependencyProperty PressedImageUriProperty =
            DependencyProperty.Register("PressedImageUri", typeof(Uri), typeof(ThreeStateImageButton), new PropertyMetadata(null));

        public Uri PressedImageUri
        {
            get { return (Uri)GetValue(PressedImageUriProperty); }
            set { SetValue(PressedImageUriProperty, value); }
        }

        public static readonly DependencyProperty DisabledImageUriProperty =
            DependencyProperty.Register("DisabledImageUri", typeof(Uri), typeof(ThreeStateImageButton), new PropertyMetadata(null));

        public Uri DisabledImageUri
        {
            get { return (Uri)GetValue(DisabledImageUriProperty); }
            set { SetValue(DisabledImageUriProperty, value); }
        }

        private Image threeStateImage;

        public ThreeStateImageButton()
        {
            this.DefaultStyleKey = typeof(ThreeStateImageButton);
        }

        protected override void OnApplyTemplate()
        {
            OnApplyTemplate();
            threeStateImage = GetTemplateChild("ThreeStateImage") as Image;
            if (threeStateImage != null)
            {
                Uri imageUri = null;

                if (this.IsEnabled == false)
                {
                    imageUri = DisabledImageUri;
                }
                else
                {
                    imageUri = NormalImageUri;
                }

                threeStateImage.Source = new BitmapImage() { UriSource = imageUri };
            }
        }

        public void SetCurrentImageUri(Uri imageUri)
        {
            if (threeStateImage != null)
            {
                threeStateImage.Source = new BitmapImage() { UriSource = imageUri };
            }
        }
    }
}
