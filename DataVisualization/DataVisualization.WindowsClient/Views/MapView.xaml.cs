using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Projection;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using Geometry = ESRI.ArcGIS.Client.Geometry.Geometry;

namespace DataVisualization.WindowsClient.Views {
    public partial class MapView : UserControl {
        private GraphicsLayer _graphicsLayer;

        private readonly Geometry _rotterdamView = new Envelope(GetPoint(4.667061, 51.950285),
            GetPoint(4.325111, 51.854432));

        private static readonly Symbol sym = new SimpleMarkerSymbol() {
            Color = new SolidColorBrush(Color.FromArgb(100, 63, 127, 191)),
            Size = 15,
            Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
        };

        public MapView() {
            InitializeComponent();
            _graphicsLayer = EsriMap.Layers["GraphicsLayer"] as GraphicsLayer;
            EsriMap.Layers.LayersInitialized += Loaded;
        }
        private void Loaded(object sender, EventArgs e) {
            EsriMap.ZoomTo(_rotterdamView);
            new Task(Async).Start();
            return;

            Symbol sym = new SimpleMarkerSymbol() {
                Color = new SolidColorBrush(Color.FromArgb(100, 63, 127, 191)),
                Size = 15,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };

            /*Graphic grx = new Graphic() {
                Geometry = GetPoint("4.4777325", "51.9244201"),
                Symbol = sym
            };
            Graphic grz = new Graphic() {
                Geometry = GetPoint("4.4777325", "51.9244215"),
                Symbol = sym
            };

            _graphicsLayer.Graphics.Add(grx);
            _graphicsLayer.Graphics.Add(grz);

            return;*/
            using (ProjectEntities db = new ProjectEntities()) {
                var res = from x in db.twitter_tweets
                    where x.coordinates_lat != null && x.coordinates_lon != null
                    select new { x.coordinates_lat, x.coordinates_lon };

                foreach (var element in res) {
                    Graphic gr = new Graphic() {
                        Geometry = GetPoint(element.coordinates_lon, element.coordinates_lat),
                        Symbol = sym
                    };
                        _graphicsLayer.Graphics.Add(gr);
                }
            }
            GeometryService service = new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");


            /*MapPoint point = GetPoint(4.5225615, 51.8550756);
            point.SpatialReference = _graphicsLayer.SpatialReference;

            _graphicsLayer.Graphics.Add(new Graphic() { 
                Geometry = point,
                Symbol = (Symbol)Resources["TestResource"]
                
            });*/
        }


        private static MapPoint GetPoint(string lon, string lat) {
            return GetPoint(double.Parse(lon), double.Parse(lat));
        }
        private static MapPoint GetPoint(double lon, double lat) {
            var merc = new WebMercator();
            var mp = merc.FromGeographic(new MapPoint(lon, lat)) as MapPoint;
            return mp;
        }

        private void Async() {

            using (ProjectEntities db = new ProjectEntities()) {
                var res = from x in db.twitter_tweets
                          where x.coordinates_lat != null && x.coordinates_lon != null
                          select new MapPointString() { LatString = x.coordinates_lat, LongString = x.coordinates_lon };
                foreach (var element in res) {
                    lock (_graphicsLayer) {
                        _graphicsLayer.Dispatcher.Invoke(() => { Add(element); });
                        Thread.Sleep(750);
                    }

                }
            }
        }

        private void Add(MapPointString data) {
            Symbol sym = new SimpleMarkerSymbol() {
                Color = new SolidColorBrush(Color.FromArgb(100, 63, 127, 191)),
                Size = 15,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };
            Graphic gr = new Graphic() {
                    Geometry = GetPoint(data.Long,data.Lat),
                    Symbol = sym
                };
                _graphicsLayer.Graphics.Add(gr);
        }
    }

    class MapPointString {
        public string LongString { get; set; }
        public string LatString { get; set; }

        public double Long => double.Parse(LongString);
        public double Lat => double.Parse(LatString);
    }
}
