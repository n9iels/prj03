using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Projection;
using ESRI.ArcGIS.Client.Symbols;

namespace DataVisualization.WindowsClient.ViewModels.MapViewModels {
    public class HeatPoint {

        public MapPoint Position { get; set; }
        public Graphic Graphic { get; set; }
        public SimpleMarkerSymbol Symbol => (SimpleMarkerSymbol) Graphic.Symbol;

        private double _size = 1.0;

        public double Size {
            get { return _size; }
            set {
                _size = value >= 3 ? 3 : value;
                Symbol.Size = DefaultSize * _size;
            }
        }

        public SolidColorBrush ColorBrush {
            get { return (SolidColorBrush) Symbol.Color; }
            set { Symbol.Color = value; }
        }

        private const double MergetTreshold = 100.0;
        private const double DefaultSize = 15;

        #region Constructors

        public HeatPoint(string longtitude, string latitude) : this(GetPosition(longtitude, latitude)) { }
        public HeatPoint(double longtitude, double latitude) : this(GetPosition(longtitude, latitude)) { }

        public HeatPoint(MapPoint point) {
            Position = point;
            Graphic = new Graphic() {
                Geometry = Position,
                Symbol =
                  new SimpleMarkerSymbol {
                      Color = new SolidColorBrush(Color.FromArgb(100, 63, 127, 191)),
                      Size = 15,
                      Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                  }
            };
        }

        #endregion

        #region Public Methods

        public void Expand() {
            Expand(1.05);
        }

        public void Expand(double factor) {
            Symbol.Size *= factor;
            SolidColorBrush brush = (SolidColorBrush) Symbol.Color;
            brush.Add(0, 15, -15, -15);
            Size *= factor;
        }

        public bool Intersects(HeatPoint other) {
            return Position.X - MergetTreshold * Size <= other.Position.X &&
                   Position.X + MergetTreshold * Size >= other.Position.X &&
                   this.Position.Y - MergetTreshold * Size <= other.Position.Y &&
                   Position.Y + MergetTreshold * Size >= other.Position.Y;
        }

        public static MapPoint GetPosition(string longtitude, string latitude) {
            return GetPosition(double.Parse(longtitude), double.Parse(latitude));
        }

        public static MapPoint GetPosition(double longtitude, double latitude) {
            WebMercator merc = new WebMercator();
            return (MapPoint) merc.FromGeographic(new MapPoint(longtitude, latitude));
        }

        #endregion
    }
}
