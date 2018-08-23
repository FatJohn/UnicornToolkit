// Copyright (c) 2018 John Shu

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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Unicorn
{
    public static class ImageExtension
    {
        public static void SetImageUri(this Image image, string imageUriString)
        {
            if (string.IsNullOrEmpty(imageUriString))
            {
                return;
            }

            if (Uri.TryCreate(imageUriString, UriKind.RelativeOrAbsolute, out Uri imageUri) == false)
            {
                return;
            }

            image.SetImageUri(imageUri);
        }

        public static void SetImageUri(this Image image, Uri imageUri)
        {
            try
            {
                if (image == null || imageUri == null)
                {
                    return;
                }

                if (image.Source is BitmapImage bitmapImage)
                {
                    if (Uri.Compare(bitmapImage.UriSource, imageUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return;
                    }
                }

                image.Source = new BitmapImage()
                {
                    UriSource = imageUri
                };
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
        }

        public static void SetImageUri(this BitmapImage image, string imageUriString)
        {
            if (image == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(imageUriString))
            {
                return;
            }

            if (Uri.TryCreate(imageUriString, UriKind.RelativeOrAbsolute, out Uri imageUri) == false)
            {
                return;
            }

            image.SetImageUri(imageUri);
        }

        public static void SetImageUri(this BitmapImage image, Uri imageUri)
        {
            try
            {
                if (image == null || imageUri == null)
                {
                    return;
                }

                if (Uri.Compare(image.UriSource, imageUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return;
                }

                image.UriSource = imageUri;
            }
            catch (Exception ex)
            {
                PlatformService.Log?.Error(ex);
            }
        }
    }
}
