using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Quickbeam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private EmbeddedWindow _haloWindow;
        private bool restoreIfMove;

        public MainWindow()
        {
            InitializeComponent();

            // For whatever reason, the default behavior of a Metro window does
            // not allow you to 'unsnap' from a maximized state by dragging the
            // titlebar. This allows such behavior.
            #region JankyMouseHandlers
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    if ((ResizeMode == ResizeMode.CanResize) ||
                        (ResizeMode == ResizeMode.CanResizeWithGrip))
                    {
                        SwitchState();
                    }
                }
                else
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        restoreIfMove = true;
                    }
                }
            };

            this.MouseLeftButtonUp += (s, e) =>
            {
                restoreIfMove = false;
            };

            this.MouseMove += (s, e) =>
            {
                if (restoreIfMove)
                {
                    restoreIfMove = false;
                    var mouseX = e.GetPosition(this).X;
                    var width = RestoreBounds.Width;
                    var x = mouseX - width / 2;

                    WindowState = WindowState.Normal;
                    Left = x;
                    Top = 36;
                    DragMove();
                }
            };
            #endregion JankyMouseHandlers
        }

        private void SwitchState()
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;

            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            _haloWindow = new EmbeddedWindow(this,
                @"D:\Program Files (x86)\Microsoft Games\Halo\halo.exe",
                @"D:\Program Files (x86)\Microsoft Games\Halo\",
                @"-console -window -vidmode 800,600,60",
                "Halo");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (_haloWindow != null)
            {
                _haloWindow.Dispose();
            }
        }
    }
}
