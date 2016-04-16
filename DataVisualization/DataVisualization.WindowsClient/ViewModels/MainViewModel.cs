using System.Windows;
using System.Windows.Input;
using DataVisualization.Data.Models.MainModel;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels {
    public class MainViewModel : CloseableViewModel {

        private readonly MainModel _mainModel;
        public MainViewModel() {
            _mainModel = new MainModel();
            CurrentView = new HomeViewModel();
        }

        #region Model get / set

        public ViewModelBase CurrentView {
            get { return _mainModel.CurrentPage; }
            set {
                _mainModel.CurrentPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand DisplayHomeCommand => new DelegateCommand(x => CurrentView = new HomeViewModel());
        public ICommand DisplayPieChartCommand => new DelegateCommand(x => CurrentView = new PieChartViewModel());
        public ICommand DisplayLineGraphCommand => new DelegateCommand(x => CurrentView = new LineGraphViewModel());
        public ICommand DisplayGaugeChartCommand => new DelegateCommand(x => CurrentView = new GaugeChartViewModel());
        public ICommand DisplayHeatmapCommand => new DelegateCommand(x => CurrentView = new MapViewModel());
        public ICommand DisplayHistogramCommand => new DelegateCommand(x => CurrentView = new ProfileChartViewModel());

        public ICommand MinimizeCommand
            => new DelegateCommand(x => Application.Current.MainWindow.WindowState = WindowState.Minimized);

        public ICommand MaximizeCommand
            =>
                new DelegateCommand(
                    (x) =>
                        Application.Current.MainWindow.WindowState =
                            Application.Current.MainWindow.WindowState == WindowState.Maximized
                                ? WindowState.Normal
                                : WindowState.Maximized);
        public ICommand ExitCommand => new DelegateCommand((x) => OnClosingRequest());
        public ICommand DragMoveCommand => new DelegateCommand(x => Application.Current.MainWindow.DragMove());

        #endregion
    }
}
