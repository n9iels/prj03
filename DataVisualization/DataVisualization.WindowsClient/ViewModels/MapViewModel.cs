using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataVisualization.Data.Models.MapModel;
using DataVisualization.Windows;
using DataVisualization.WindowsClient.ViewModels.MapViewModels;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

namespace DataVisualization.WindowsClient.ViewModels {
    public class MapViewModel : ViewModelBase {
        
        private readonly MapModel _model;
        private readonly List<HeatPoint> _heatPoints;
        private static readonly Geometry RotterdamView = new Envelope(HeatPoint.GetPosition(4.667061, 51.950285),
    HeatPoint.GetPosition(4.325111, 51.854432));

        private Thread _visualizationThread;
        public MapViewModel() {
            _model = new MapModel();
            _heatPoints = new List<HeatPoint>();
            HeatMap = (Map)Application.Current.FindResource("HeatMap");
            HeatMap.Layers.LayersInitialized += delegate { HeatMap.ZoomTo(RotterdamView); };
            Speed = 1;

            using (ProjectEntities db = new ProjectEntities()) {
                var all = from row in db.twitter_tweets select row.created_at;
                FirstAvailableDate = all.OrderBy(x => x).Take(1).Single();
            }
            LastAvailableDate = DateTime.Now;
            StartDate = FirstAvailableDate;
            EndDate = LastAvailableDate;
        }

        public ICommand StartVisualizationCommand
            =>
                new DelegateCommand(x => (_visualizationThread = new Thread(StartMapAnimation)).Start(),
                    x => _visualizationThread == null || !_visualizationThread.IsAlive);

        public ICommand IncreaseSpeedCommand => new DelegateCommand(x => Speed += 1);
        public ICommand DecreaseSpeedCommand => new DelegateCommand(x => Speed -= 1);

        private void StartMapAnimation() {
            // Clear all current points
            Graphics.Dispatcher.Invoke(() => {
                Graphics.Graphics.Clear();
                _heatPoints.Clear();
            });


            using (ProjectEntities db = new ProjectEntities()) {
                var data = from x in db.twitter_tweets
                    where
                        x.coordinates_lat != null && x.coordinates_lon != null && x.created_at >= StartDate &&
                        x.created_at <= EndDate
                    orderby x.created_at
                    select new { Time = x.created_at, Latitude = x.coordinates_lat, Longtitude = x.coordinates_lon };

                var enumer = data.GetEnumerator();
                enumer.MoveNext();

                // No data available
                if (enumer.Current == null) {
                    return;
                }


                CurrentTime = enumer.Current.Time;

                while (true) {
                    bool animationDone = false;

                    CurrentTime = ((DateTime)CurrentTime).Add(new TimeSpan(0, 1, 0));
                    try {
                        Graphics.Dispatcher.Invoke(() => {
                            while (enumer.Current.Time < CurrentTime) {
                                HeatPoint point = new HeatPoint(enumer.Current.Longtitude, enumer.Current.Latitude);
                                HeatPoint intersect = _heatPoints.FirstOrDefault(x => x.Intersects(point));
                                if (intersect != null)
                                    intersect.Expand();
                                else {
                                    _heatPoints.Add(point);
                                    Graphics.Graphics.Add(point.Graphic);
                                }
                                if (!enumer.MoveNext()) {
                                    animationDone = true;
                                    break;
                                }
                            }
                        });
                    }
                    catch (TaskCanceledException) {
                        return;
                    }
                    if (animationDone) {
                        return;
                    }
                    Thread.Sleep((int)(10 / Speed));
                }
            }
        }


        #region Data Binding
        public DateTime? CurrentTime {
            get { return _model.CurrentTime; }
            set {
                _model.CurrentTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate {
            get { return _model.StartDate; }
            set {
                _model.StartDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate {
            get { return _model.EndDate; }
            set {
                _model.EndDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime FirstAvailableDate
        {
            get { return _model.FirstAvailableDate; }
            set
            {
                _model.FirstAvailableDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastAvailableDate
        {
            get { return _model.LastAvailableDate; }
            set
            {
                _model.LastAvailableDate = value;
                OnPropertyChanged();
            }
        }

        public Map HeatMap {
            get { return _model.HeatMap;}
            set {
                _model.HeatMap = value;
                Graphics = (GraphicsLayer) _model.HeatMap.Layers["GraphicsLayer"];
                OnPropertyChanged();
            }
        }

        public double Speed {
            get { return _model.Speed; }
            set {
                if (value > 10)
                    value = 10;
                if (value < 1)
                    value = 1;
                _model.Speed = value;
                OnPropertyChanged();
            }
        }

        public GraphicsLayer Graphics { get; set; }

        #endregion
    }
}