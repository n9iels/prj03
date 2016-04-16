using System.Windows.Media;

namespace DataVisualization.WindowsClient.ViewModels.MapViewModels {
    public static class ColorExtensions {

        public static void Add(this SolidColorBrush original, int a, int r, int g, int b) {
            original.Color = Color.FromArgb((original.Color.A + a).ByteClamp(), (original.Color.R + r).ByteClamp(),
                (original.Color.G + g).ByteClamp(), (original.Color.B + b).ByteClamp());

        }

        private static byte ByteClamp(this int value) {
            return value >= 255 ? (byte)255 : value <= 0 ? (byte)0 : (byte)value;
        }

    }
}
