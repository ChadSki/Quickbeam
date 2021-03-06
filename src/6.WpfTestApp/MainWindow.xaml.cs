﻿using NimbusSharp;
using NimbusSharpGUI.TagEditor;
using System.Windows;

namespace WpfTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var map = Workbench.Instance.OpenMap();
            var tag = map.ArbitraryTag;
            treeView.Root = new HaloTagNode(tag);
        }
    }
}
