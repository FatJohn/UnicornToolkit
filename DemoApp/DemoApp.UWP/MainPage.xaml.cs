using System;
using System.Threading.Tasks;
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

        public async Task Login()
        {
            var parameter = new LoginParameter
            {
                Date = DateTime.Now,
                UserId = "myUserId",
                Password = "myPassword",
            };

            var service = new LoginService();
            var result = await service.InvokeAsync(parameter);
        }
    }
}
