using System;
using Unicorn;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoApp.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //var parameter = new LoginParameter
            //{
            //    Date = DateTime.Now,
            //    UserId = "myUserId",
            //    Password = "myPassword",
            //};

            //var service = new LoginService();
            //var result = await service.InvokeAsync(parameter);

            var parameter = new GoogleSearchParameter
            {
                Timeout = TimeSpan.FromMilliseconds(100),
                q = "我是誰",
            };

            var service = new GoogleSearchService();
            var result = await service.InvokeAsync(parameter);

            System.Diagnostics.Debug.WriteLine(result.Content);


            //await StorageHelper.CopyFolderFromInstalledLocation("assets");
            //await StorageHelper.GetFilesInFolder("assets\\LockScreenLogo.scale-200.png", ApplicationData.Current.LocalFolder);
            //await StorageHelper.CopyFolder(ApplicationData.Current.LocalFolder, ApplicationData.Current.LocalFolder);
        }
    }
}
