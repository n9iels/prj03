using DataVisualization.Data.Models.LineGraphModel;
using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.WindowsClient.ViewModels.LineGraphs;

namespace DataVisualization.WindowsClient.ViewModels
{
    public class LineGraphViewModel : ViewModelBase
    {
        private readonly LineGraphModel _model;

        public LineGraphViewModel()
        {
            _model = new LineGraphModel();
            SelectTemperatureCommand.Execute(null);
        }

        public ViewModelBase CurrentGraph
        {
            get { return _model.CurrentGraph; }
            set
            {
                _model.CurrentGraph = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectTemperatureCommand => new DelegateCommand(x => CurrentGraph = new TemperatureViewModel());
        public ICommand SelectPindexCommand => new DelegateCommand(x => CurrentGraph = new PindexViewModel());
        public ICommand SelectDayCommand => new DelegateCommand(x => CurrentGraph = new DayViewModel());
    }
}
