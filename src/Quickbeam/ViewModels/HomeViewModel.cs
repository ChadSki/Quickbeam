using Quickbeam.Interfaces;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Quickbeam.ViewModels
{
    public class HomeViewModel : Base
    {
        public HomeViewModel()
        {
            StatusResetTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 5) };
            StatusResetTimer.Tick += (sender, args) => { Status = "Ready..."; };
        }

        #region Properties

        #region Application Stuff

        private DispatcherTimer StatusResetTimer { get; set; }

        private string _status = "Ready...";
        public string Status
        {
            get { return _status; }
            set
            {
                if (!value.EndsWith("..."))
                    value += "...";

                StatusResetTimer.Stop();
                SetField(ref _status, value);
                StatusResetTimer.Start();
            }
        }

        private Thickness _applicationBorderThickness;
        public Thickness ApplicationBorderThickness
        {
            get { return _applicationBorderThickness; }
            set { SetField(ref _applicationBorderThickness, value); }
        }

        #endregion

        #region Window Masks

        private Visibility _maskVisibility = Visibility.Collapsed;

        public Visibility MaskVisibility
        {
            get { return _maskVisibility; }
            set { SetField(ref _maskVisibility, value); }
        }

        #endregion

        #region Window Actions/Resizing

        private Visibility _actionMaximizeVisibility = Visibility.Collapsed;
        private Visibility _actionRestoreVisibility = Visibility.Collapsed;
        private Visibility _resizingVisibility = Visibility.Collapsed;

        public Visibility ActionRestoreVisibility
        {
            get { return _actionRestoreVisibility; }
            set { SetField(ref _actionRestoreVisibility, value); }
        }

        public Visibility ActionMaximizeVisibility
        {
            get { return _actionMaximizeVisibility; }
            set { SetField(ref _actionMaximizeVisibility, value); }
        }

        public Visibility ResizingVisibility
        {
            get { return _resizingVisibility; }
            set { SetField(ref _resizingVisibility, value); }
        }

        #endregion

        #region Content

        private IView _assemblyPage;

        public IView MainPage
        {
            get { return _assemblyPage; }
            set
            {
                // try closing current page
                if (_assemblyPage != null && !_assemblyPage.Close())
                    return;

                // aite, we can
                SetField(ref _assemblyPage, value);
            }
        }

        #endregion

        #endregion

        #region Events

        #region Overrides

        public void OnStateChanged(object sender, EventArgs eventArgs)
        {
            switch ((WindowState) sender)
            {
                case WindowState.Normal:
                    ApplicationBorderThickness = new Thickness(1, 1, 1, 23);
                    ActionRestoreVisibility = Visibility.Collapsed;
                    ActionMaximizeVisibility =
                        ResizingVisibility = Visibility.Visible;
                    break;
                case WindowState.Maximized:
                    ApplicationBorderThickness = new Thickness(0, 0, 0, 23);
                    ActionRestoreVisibility = Visibility.Visible;
                    ActionMaximizeVisibility =
                        ResizingVisibility = Visibility.Collapsed;
                    break;
            }
        }

        #endregion

        #endregion

        #region Dialog Management

        private int _maskCount;

        public void ShowDialogMask(bool showMask = true)
        {
            if (showMask)
                _maskCount++;

            if (_maskCount > 0)
                MaskVisibility = Visibility.Visible;
        }

        public void HideDialogMask(bool maskShown = true)
        {
            if (maskShown)
                _maskCount--;

            if (_maskCount == 0)
                MaskVisibility = Visibility.Collapsed;
        }

        #endregion

    }
}