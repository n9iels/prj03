using System;

namespace DataVisualization.Windows {
    public class CloseableViewModel : ViewModelBase {

        public event EventHandler ClosingRequest;

        protected void OnClosingRequest() {
            this.ClosingRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
