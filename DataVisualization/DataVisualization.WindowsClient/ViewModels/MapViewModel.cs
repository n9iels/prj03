using System;
using DataVisualization.Windows;
using DataVisualization.WindowsClient.ViewModels.MapViewModels;

namespace DataVisualization.WindowsClient.ViewModels {
    public class MapViewModel : ViewModelBase {

        private readonly MapModel _model;
        public MapViewModel() {
            _model = new MapModel();
            CurrentTime = new DateTime(2016, 4, 17, 17, 22, 30);
        }

        public DateTime CurrentTime {
            get { return _model.CurrentTime; }
            set {
                _model.CurrentTime = value;
                OnPropertyChanged();
            }
        }
    }
}