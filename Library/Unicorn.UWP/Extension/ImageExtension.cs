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
    }
}
