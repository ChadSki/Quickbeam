using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Quickbeam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private EmbeddedWindow haloWindow;
        private EmbeddedWindow st2Window;
        private bool restoreIfMove;

        public static Color blue = Color.FromRgb(0x00, 0x71, 0xcc);
        public static Color purple = Color.FromRgb(0x68, 0x21, 0x7a);

        public MainWindow()
        {
            InitializeComponent();
            statusColorBrush.Color = purple;
            widthSlider.Value = 800;
            heightSlider.Value = 600;

            // For whatever reason, the default behavior of a Metro window does
            // not allow you to 'unsnap' from a maximized state by dragging the
            // titlebar. This allows such behavior.
            #region JankyMouseHandlers
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ClickCount == 2)
                {
                    SwitchState();
                }
                else
                {
                    if (WindowState == WindowState.Maximized)
                        restoreIfMove = true;
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

                    // Stay within the work area when we unsnap from maximized
                    if (x < 0)
                        x = 0;
                    if (x > SystemParameters.WorkArea.Width - width)
                        x = SystemParameters.WorkArea.Width - width;

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

        private void btnHost_Click(object s, RoutedEventArgs e)
        {
            var width = Convert.ToInt32(widthSlider.Value);
            var height = Convert.ToInt32(heightSlider.Value);
            var st2Width = Convert.ToInt32(st2Container.ActualWidth);

            haloWindow = new EmbeddedWindow(this,
                @"D:\Program Files (x86)\Microsoft Games\Halo\halo.exe",
                @"D:\Program Files (x86)\Microsoft Games\Halo\",
                string.Format(@"-console -window -vidmode {0},{1},60", width, height),
                "Halo");
            haloWindow.Resize(width, height, 7, 44);

            st2Window = new EmbeddedWindow(this,
                @"D:\Users\Chad\Dropbox\Workbench\CodeProjects\Halo\Quickbeam\bin\Debug\Sublime Text 2.0.2 x64\sublime_text.exe",
                //@"C:\Dropbox\Workbench\CodeProjects\Halo\Quickbeam\bin\Debug\Sublime Text 2.0.2 x64\sublime_text.exe",
                @"D:\Users\Chad\Dropbox\Workbench\CodeProjects\Halo\halolib.py\",
                //@"C:\Dropbox\Workbench\CodeProjects\Halo\halolib.py",
                null,
                "untitled - Sublime Text 2 (UNREGISTERED)");
            st2Window.Resize(st2Width, height, 7 + width + 6, 44);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (haloWindow != null)
            {
                haloWindow.Dispose();
            }
        }
    }
}
