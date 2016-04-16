using System.Windows;
using DataVisualization.WindowsClient.ViewModels;
using DataVisualization.WindowsClient.Views;

namespace DataVisualization.WindowsClient {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {


        protected override void OnStartup(StartupEventArgs e) {
            var vm = new MainViewModel();
            var view = new MainView() { DataContext = vm };
            vm.ClosingRequest += delegate { view.Close(); };
            view.Show();
        }
    }
}
