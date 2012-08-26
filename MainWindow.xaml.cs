using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Quickbeam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HaloMap _haloMap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _haloMap = new HaloMap(); // Empty
        }

        private void UpdateContent()
        {
            TagTree.Items.Clear();
            if (_haloMap.TreeViewTagClasses != null)
            {
                foreach (TreeViewItem tvm in _haloMap.TreeViewTagClasses)
                {
                    TagTree.Items.Add(tvm);
                    Console.WriteLine(tvm.Header);
                }
            }
            List<string> tagClasses = _haloMap.TagsByClass.Keys.ToList();
            tagClasses.Sort();
            foreach (string tagClass in tagClasses)
            {
                TreeViewItem classNode = new TreeViewItem();
                classNode.Header = tagClass;

                IEnumerable<HaloTag> tagsOfCurrentClass = _haloMap.TagsByClass[tagClass];
                foreach (HaloTag ht in tagsOfCurrentClass)
                {
                    TreeViewItem tagNode = new TreeViewItem();
                    tagNode.Header = ht.Name;
                    classNode.Items.Add(tagNode);
                }

                TagTree.Items.Add(classNode);
            }

            ReferencesList.Items.Clear();
            if (_haloMap.TagsInOrder != null)
                foreach (HaloTag ht in _haloMap.TagsInOrder)
                    ReferencesList.Items.Add(ht.Name);
        }

        /// <summary>
        /// The dialog for opening a mapfile.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = @"D:\Programs\Microsoft Games\Halo\MAPS"; //TODO Make this be loaded via settings file
            dlg.FileName = "bloodgulch.map"; // Default file name
            dlg.DefaultExt = ".map"; // Default file extension
            dlg.Filter = "Halo mapfiles|*.map"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                _haloMap.LoadMap(dlg.FileName);
                UpdateContent();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Written by WaeV");
        }
    }
}