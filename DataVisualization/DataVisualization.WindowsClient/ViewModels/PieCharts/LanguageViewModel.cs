using DataVisualization.Data.Models.PieChartModel.PieCharts;
using DataVisualization.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataVisualization.WindowsClient.ViewModels.PieCharts
{
    public class LanguageViewModel : ViewModelBase
    {
        public ObservableCollection<LanguageModel> Data { get; private set; }

        public LanguageViewModel()
        {
            RefreshCommand.Execute(null);
        }

        public void RefreshChart()
        {
            const double factor = 0.009;

            // Last 45 minutes
            DateTime time = DateTime.UtcNow - new TimeSpan(0, 0, 45, 0);

            using (ProjectEntities db = new ProjectEntities())
            {

                var first = from t in db.twitter_tweets
                                //where t.created_at > time
                            group t by t.language
                    into lang
                            where lang.Key != null && lang.Key != "UN_NotReferenced"
                            select new { Language = lang, Count = lang.Count() };

                double sum = (from all in first select all.Count).Sum();


                var highCount = from entry in first
                                where entry.Count >= sum * factor
                                select new LanguageModel() { Category = entry.Language.Key, Number = entry.Language.Count() };

                var lowCount =
                    first.Select(
                        x =>
                            new LanguageModel()
                            {
                                Category = "Other",
                                Number = first.Where(y => y.Count < sum * factor).Sum(z => z.Count)
                            });

                var res = highCount.Concat(lowCount);

                Data = new ObservableCollection<LanguageModel>(res);
                OnPropertyChanged(nameof(Data));
            }
        }

        public ICommand RefreshCommand => new DelegateCommand((x) => new Task(RefreshChart).Start());
    }
}
