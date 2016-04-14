using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Data.Models.PieChartModel {
    public class PieChartModel {

        public ObservableCollection<PieChartContent> ChartData { get; set; }
        public object SelectedChartItem { get; set; } 

        public Action CurrentChartCommand { get; set; }
    }
}
