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
        private List<HeatPoint> HeatPoints = new List<HeatPoint>(); 

        private readonly Geometry _rotterdamView = new Envelope(GetPoint(4.667061, 51.950285),
            GetPoint(4.325111, 51.854432));

        public MapView() {
            InitializeComponent();
            _graphicsLayer = EsriMap.Layers["GraphicsLayer"] as GraphicsLayer;
            EsriMap.Layers.LayersInitialized += LoadedMap;
        }
        private void LoadedMap(object sender, EventArgs e) {
            EsriMap.ZoomTo(_rotterdamView);
            //new Task(MergeTest).Start();
            Thread x = new Thread(new ThreadStart(MergeTest));
            x.Start();
            return;
            GeometryService service = new GeometryService("https://utility.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");

        }

        private void MergeTest() {

            Tuple<double, double>[] arr = {
                Tuple.Create(4.4777325, 51.9244201),
                Tuple.Create(4.4777325, 51.9244215),
                Tuple.Create(4.4777325, 51.9244235),
                Tuple.Create(4.4777325, 51.9244245),
            };

            foreach (var element in arr) {
                _graphicsLayer.Dispatcher.Invoke(() => {
                    HeatPoint point = new HeatPoint(element.Item1, element.Item2);
                    HeatPoint intersect = HeatPoints.FirstOrDefault(x => x.Intersects(point));
                    if (intersect != null)
                        intersect.Expand();
                    else {
                        _graphicsLayer.Graphics.Add(point.Graphic);
                        HeatPoints.Add(point);
                    }
                });
                Thread.Sleep(2500);
            }
        }


        private void LiveMergeTest() {
            /*using (ProjectEntities db = new ProjectEntities()) {
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
                            //_graphicsLayer.Dispatcher.Invoke(() => { Add(element); });
  
                        }
                    Thread.Sleep(500);
                }
            }*/
        }

        private static MapPoint GetPoint(string lon, string lat) {
            return GetPoint(double.Parse(lon), double.Parse(lat));
        }
        private static MapPoint GetPoint(double lon, double lat) {
            var merc = new WebMercator();
            var mp = merc.FromGeographic(new MapPoint(lon, lat)) as MapPoint;
            return mp;
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
        public Graphic Graphic { get; set; }
        public SimpleMarkerSymbol Symbol => (SimpleMarkerSymbol) Graphic.Symbol;

        public double Size { get; set; } = 1.0;

        private const double MergetTreshold = 1.0;

        #region Constructors

        public HeatPoint(double longtitude, double latitude) {
            WebMercator mec = new WebMercator();
            Position = (MapPoint) mec.FromGeographic(new MapPoint(longtitude, latitude));
            Graphic = new Graphic() {
                Geometry = Position,
                Symbol = new SimpleMarkerSymbol {
                    Color = new SolidColorBrush(Color.FromArgb(100,63,127,191)),
                    Size = 15,
                    Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                }
            };

        }
        #endregion

        public void Expand() { Expand(1.2); }

        public void Expand(double factor) {
            Symbol.Size *= 1.3;
            SolidColorBrush brush = (SolidColorBrush)Symbol.Color;
            brush.Color = Color.FromArgb(brush.Color.A, (byte)(brush.Color.R + 55), brush.Color.G,
                (byte)(brush.Color.B - 15));
            Size *= 1.2;
        }

        public bool Intersects(HeatPoint other) {
            return Position.X - MergetTreshold * Size <= other.Position.X &&
                   Position.X + MergetTreshold * Size >= other.Position.X &&
                   this.Position.Y - MergetTreshold * Size <= other.Position.Y &&
                   Position.Y + MergetTreshold * Size >= other.Position.Y;
        }



    }
}
