using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataVisualization.Windows {

    public abstract class ViewModelBase : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, e);
        }
    }
}
