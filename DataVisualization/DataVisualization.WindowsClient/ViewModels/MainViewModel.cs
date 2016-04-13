using System.Windows.Input;
using DataVisualization.Data.Models.MainModel;
using DataVisualization.Windows;

namespace DataVisualization.WindowsClient.ViewModels {
    public class MainViewModel : ViewModelBase {

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
        public ICommand DisplayGraphCommand => new DelegateCommand(x => CurrentView = new PieChartViewModel());

        #endregion
    }
}
