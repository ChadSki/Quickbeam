using Quickbeam.Interfaces;
using Quickbeam.Native;
using System.Windows;

namespace Quickbeam.ViewModels
{
    public class MainPageViewModel : Base
    {
        private GridLength _haloWidth = new GridLength(0, GridUnitType.Auto);
        private GridLength _haloHeight = new GridLength(0, GridUnitType.Auto);

        public static string HaloExePath { get { return HaloSettings.HaloExePath; } }
        public static string HaloVersion { get { return HaloSettings.HaloVersion; } }

        public GridLength HaloGridWidth
        {
            get
            {
                if (_haloWidth.IsAuto)
                    HaloWidth = DefaultWidth;
                return _haloWidth;
            }
            set
            {
                _haloWidth = value;
                OnPropertyChanged("HaloWidth");
            }
        }

        public GridLength HaloGridHeight
        {
            get
            {
                if (_haloHeight.IsAuto)
                    HaloHeight = DefaultHeight;
                return _haloHeight;
            }
            set
            {
                _haloHeight = value;
                OnPropertyChanged("HaloHeight");
            }
        }

        private const int DefaultWidth = 640;
        private const int DefaultHeight = 480;
        private const int PixelsBorder = 4;

        public int HaloWidth
        {
            get { return DpiConversion.PointsToPixels(_haloWidth.Value, DpiConversion.Direction.Horizontal) - PixelsBorder; }
            set
            {
                SetField(ref _haloWidth,
                    new GridLength(DpiConversion.PixelsToPoints(value + PixelsBorder, DpiConversion.Direction.Horizontal), GridUnitType.Pixel));
                OnPropertyChanged("HaloGridWidth");
            }
        }

        public int HaloHeight
        {
            get { return DpiConversion.PointsToPixels(_haloHeight.Value, DpiConversion.Direction.Horizontal) - PixelsBorder; }
            set
            {
                SetField(ref _haloHeight,
                    new GridLength(DpiConversion.PixelsToPoints(value + PixelsBorder, DpiConversion.Direction.Horizontal), GridUnitType.Pixel));
                OnPropertyChanged("HaloGridHeight");
            }
        }
    }
}
