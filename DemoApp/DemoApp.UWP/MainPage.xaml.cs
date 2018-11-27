using System;
using System.Threading.Tasks;
using Unicorn.ServiceModel;
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
        }
    }
}
