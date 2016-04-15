using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Projection;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;

namespace DataVisualization.WindowsClient.Views {
    public partial class MapView : UserControl {
        private GraphicsLayer _graphicsLayer;
        private MapPoint _ptStart;
        private MapPoint _ptEnd;


        public MapView() {
            InitializeComponent();
            _graphicsLayer = EsriMap.Layers["GraphicsLayer"] as GraphicsLayer;
            EsriMap.Layers.LayersInitialized += Loaded;
        }

        private void Loaded(object sender, EventArgs e) {

            using (ProjectEntities db = new ProjectEntities()) {
                var res = from x in db.twitter_tweets
                    where x.coordinates_lat != null && x.coordinates_lon != null
                    select new { x.coordinates_lat, x.coordinates_lon };

                foreach (var element in res) {
                    _graphicsLayer.Graphics.Add(new Graphic() {
                        Geometry = GetPoint(element.coordinates_lon, element.coordinates_lat),
                        Symbol = (Symbol)Resources["TestResource"]
                    });
                }
            }
            GeometryService service = new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");


            Geometry newGeo = service.Union(_graphicsLayer.Graphics);

            _graphicsLayer.Graphics.Clear();
            _graphicsLayer.Graphics.Add(newGeo);

            /*MapPoint point = GetPoint(4.5225615, 51.8550756);
            point.SpatialReference = _graphicsLayer.SpatialReference;

            _graphicsLayer.Graphics.Add(new Graphic() { 
                Geometry = point,
                Symbol = (Symbol)Resources["TestResource"]
                
            });*/
        }


        private MapPoint GetPoint(string lon, string lat) {
            return GetPoint(double.Parse(lon), double.Parse(lat));
        }
        private MapPoint GetPoint(double lon, double lat) {
            var merc = new WebMercator();
            var mp = merc.FromGeographic(new MapPoint(lon, lat)) as MapPoint;
            return mp;
        }
    }
}
