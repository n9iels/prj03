using System;

namespace DataVisualization.Data.Models.PieChartModel.PieCharts {
    public class PositivityModel {

        #region Data Binding

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FirstAvailableDate { get; set; }
        public DateTime LastAvailableDate { get; set; }

        #endregion
    }
}
