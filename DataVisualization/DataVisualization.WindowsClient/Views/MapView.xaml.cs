using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataVisualization.WindowsClient.ViewModels.MapViewModels;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

namespace DataVisualization.WindowsClient.Views {
    public partial class MapView : UserControl {

        private readonly GraphicsLayer _graphicsLayer;
        private readonly List<HeatPoint> _heatPoints = new List<HeatPoint>(); 

        private readonly Geometry _rotterdamView = new Envelope(HeatPoint.GetPosition(4.667061, 51.950285),
            HeatPoint.GetPosition(4.325111, 51.854432));

        public MapView() {
            InitializeComponent();
            _graphicsLayer = EsriMap.Layers["GraphicsLayer"] as GraphicsLayer;
            EsriMap.Layers.LayersInitialized += LoadedMap;
        }
        private void LoadedMap(object sender, EventArgs e) {
            EsriMap.ZoomTo(_rotterdamView);
            new Task(LiveMergeTest).Start();
            return;
            GeometryService service = new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");

        }

        private void LiveMergeTest() {
            using (ProjectEntities db = new ProjectEntities()) {
                var res = from x in db.twitter_tweets
                          where x.coordinates_lat != null && x.coordinates_lon != null
                          select new { Latitude = x.coordinates_lat, Longtitude = x.coordinates_lon };
                foreach (var element in res) {
                    _graphicsLayer.Dispatcher.Invoke(() => {
                        HeatPoint point = new HeatPoint(element.Longtitude, element.Latitude);
                        HeatPoint intersect = _heatPoints.FirstOrDefault(x => x.Intersects(point));
                        if (intersect != null)
                            intersect.Expand();
                        else {
                            _graphicsLayer.Graphics.Add(point.Graphic);
                            _heatPoints.Add(point);
                        }
                    });
                    Thread.Sleep(15);
                }
            }
        }
    }

}
