using System;
using ESRI.ArcGIS.Client;

namespace DataVisualization.Data.Models.MapModel {
    public class MapModel {

        public DateTime? CurrentTime { get; set; }
        public Map HeatMap { get; set; }

        public double Speed { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime FirstAvailableDate { get; set; }
        public DateTime LastAvailableDate { get; set; }
    }
}
