using Quickbeam.Models;
using Quickbeam.Views;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Quickbeam.ViewModels
{
    public class HomeViewModel : Base
    {
        public HomeViewModel()
        {
            _statusResetTimer.Tick += (sender, args) => { Status = "Ready..."; };
        }

        #region Properties

        #region Application Stuff

        private readonly DispatcherTimer _statusResetTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 5)};
        private Thickness _applicationBorderThickness;

        private string _applicationTitle = "Welcome";
        private string _status = "Ready...";

        public string Status
        {
            get { return _status; }
            set
            {
                if (!value.EndsWith("..."))
                    value += "...";

                _statusResetTimer.Stop();
                SetField(ref _status, value);
                _statusResetTimer.Start();
            }
        }

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

        private IAssemblyPage _assemblyPage;

        public IAssemblyPage AssemblyPage
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

        public void ShowDialog(bool showMask = true)
        {
            if (showMask)
                _maskCount++;

            if (_maskCount > 0)
                MaskVisibility = Visibility.Visible;
        }

        public void HideDialog(bool maskShown = true)
        {
            if (maskShown)
                _maskCount--;

            if (_maskCount == 0)
                MaskVisibility = Visibility.Collapsed;
        }

        #endregion

    }
}