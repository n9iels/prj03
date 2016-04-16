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

        private const double IntersectOffset = 1.0;
        private GraphicsLayer _graphicsLayer;

        private readonly Geometry _rotterdamView = new Envelope(GetPoint(4.667061, 51.950285),
            GetPoint(4.325111, 51.854432));

        public MapView() {
            InitializeComponent();
            _graphicsLayer = EsriMap.Layers["GraphicsLayer"] as GraphicsLayer;
            EsriMap.Layers.LayersInitialized += LoadedMap;
        }
        private void LoadedMap(object sender, EventArgs e) {
            EsriMap.ZoomTo(_rotterdamView);
            //new Task(Async).Start();
            MergeTest();
            return;
            GeometryService service = new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");

        }

        private void MergeTest() {
            Graphic[] graphics = {
                new Graphic() { Geometry = GetPoint("4.4777325", "51.9244201"), Symbol = CreateSymbol() },
                new Graphic() { Geometry = GetPoint("4.4777325", "51.9244215"), Symbol = CreateSymbol() },
                new Graphic() { Geometry = GetPoint("4.4777325", "51.9244235"), Symbol = CreateSymbol() },
                new Graphic() { Geometry = GetPoint("4.4777325", "51.9244245"), Symbol = CreateSymbol() },
                 new Graphic() { Geometry = GetPoint("5.4777325", "51.9244245"), Symbol = CreateSymbol() }
            };
            foreach (var element in graphics) {
                bool intersected = false;
                for (int i = _graphicsLayer.Graphics.Count - 1; i >= 0; i--) {
                    if (element == _graphicsLayer.Graphics[i]) continue;
                    if (Intersects(element, _graphicsLayer.Graphics[i])) {
                        intersected = true;
                        SimpleMarkerSymbol targetSymbol = (SimpleMarkerSymbol)_graphicsLayer.Graphics[i].Symbol;
                        targetSymbol.Size *= 1.3;
                        SolidColorBrush brush = (SolidColorBrush) targetSymbol.Color;
                        brush.Color = Color.FromArgb(brush.Color.A, (byte)(brush.Color.R + 55), brush.Color.G, (byte)(brush.Color.B - 15));
                        Debug.WriteLine("Changed colour");
                        break;
                    }
                }
                if(!intersected)
                    _graphicsLayer.Graphics.Add(element);
            }
            GeometryService service =
                new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");
            Geometry x = service.Union(new List<Graphic>(graphics.Take(2)));

            Symbol sm = new SimpleMarkerSymbol() {
                Color = new SolidColorBrush(Color.FromArgb(60, 255,0,0)),
                Size = 15,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };
        }


        private void LiveMergeTest() {
            using (ProjectEntities db = new ProjectEntities()) {
                var res = from x in db.twitter_tweets
                          where x.coordinates_lat != null && x.coordinates_lon != null
                          select new MapPointString() { LatString = x.coordinates_lat, LongString = x.coordinates_lon };
                foreach (var element in res.Take(100)) {
                    Graphic gr = CreateGraphic(element);
                    bool intersected = false;
                    lock (_graphicsLayer.Graphics) {
                        foreach (var pnt in _graphicsLayer.Graphics.Where(x => Intersects(x, gr))) {
                            _graphicsLayer.Dispatcher.Invoke(() => {
                                SimpleMarkerSymbol targetSymbol = (SimpleMarkerSymbol) pnt.Symbol;
                                targetSymbol.Size *= 1.3;
                                SolidColorBrush brush = (SolidColorBrush) targetSymbol.Color;
                                brush.Color = Color.FromArgb(brush.Color.A, (byte) (brush.Color.R + 55), brush.Color.G,
                                    (byte) (brush.Color.B - 15));
                                intersected = true;
                            });
                            break;
                        }
                    }
                    if(!intersected)
                        lock (_graphicsLayer) {
                            _graphicsLayer.Dispatcher.Invoke(() => { Add(element); });
  
                        }
                    Thread.Sleep(500);
                }
            }
        }

        private static MapPoint GetPoint(string lon, string lat) {
            return GetPoint(double.Parse(lon), double.Parse(lat));
        }
        private static MapPoint GetPoint(double lon, double lat) {
            var merc = new WebMercator();
            var mp = merc.FromGeographic(new MapPoint(lon, lat)) as MapPoint;
            return mp;
        }

        private static bool Intersects(Graphic one, Graphic two) {
            Envelope exOne = one.Geometry.Extent;
            Envelope exTwo = two.Geometry.Extent;
            if (exOne.XMax - IntersectOffset <= exTwo.XMax && exOne.XMax + IntersectOffset >= exTwo.XMax &&
                exOne.YMax - IntersectOffset <= exTwo.YMax && exOne.YMax + IntersectOffset >= exTwo.YMax)
                return true;
            return false;
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

        private Graphic CreateGraphic(MapPointString data) {
            return new Graphic() {
                Geometry = GetPoint(data.Long, data.Lat),
                Symbol = CreateSymbol()
            };
        }

        private Symbol CreateSymbol() {
            return new SimpleMarkerSymbol() {
                Color = new SolidColorBrush(Color.FromArgb(100, 63, 127, 191)),
                Size = 15,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };
        }
    }

    class MapPointString {
        public string LongString { get; set; }
        public string LatString { get; set; }

        public double Long => double.Parse(LongString);
        public double Lat => double.Parse(LatString);
    }

    public class HeatPoint {

        public MapPoint Position { get; set; }
        public Graphic Graphic { get; }

        public HeatPoint(double longtitude, double latitude) {
            WebMercator mec = new WebMercator();
            Position = (MapPoint)mec.FromGeographic(new MapPoint(longtitude, latitude));
            Graphic = new Graphic() {
                Geometry = Position,
                Symbol = new SimpleMarkerSymbol {
                    Color = new SolidColorBrush(Color.FromArgb(100,63,127,191)),
                    Size = 15,
                    Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                }
            };

        }



    }
}
