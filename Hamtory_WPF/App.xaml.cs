using System.Configuration;
using System.Data;
using System.Windows;
using System.Threading.Tasks;

namespace Hamtory_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 비동기 초기화 호출
            await DataList.InitializeAsync();
        }
    }

}
