using System;
using ESRI.ArcGIS.Client;

namespace DataVisualization.WindowsClient.ViewModels.MapViewModels {
    public class MapModel {

        public DateTime CurrentTime { get; set; }
        public Map HeatMap { get; set; }
    }
}
