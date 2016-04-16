using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DataVisualization.Data.Models.PieChartModel;
using DataVisualization.Windows;
using DataVisualization.WindowsClient.ViewModels;
using DataVisualization.WindowsClient.ViewModels.PieCharts;

namespace DataVisualization.WindowsClient.ViewModels
{
    public class PieChartViewModel : ViewModelBase
    {
        private readonly PieChartModel _model;

        public PieChartViewModel()
        {
            _model = new PieChartModel();
            SelectLanguageCommand.Execute(null);
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

        public object SelectedItem
        {
            get { return _model.SelectedChartItem; }
            set
            {
                _model.SelectedChartItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectLanguageCommand => new DelegateCommand(x => CurrentGraph = new LanguageViewModel());
        public ICommand SelectPositivityCommand => new DelegateCommand(x => CurrentGraph = new PositivityViewModal());

    }
}