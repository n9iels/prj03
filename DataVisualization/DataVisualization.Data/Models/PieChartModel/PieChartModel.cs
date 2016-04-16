using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Data.Models.PieChartModel
{
    public class PieChartModel
    {
        public ViewModelBase CurrentGraph { get; set; }

        public object SelectedChartItem = null;
    }
}
