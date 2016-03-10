﻿using System.Windows;

namespace MetroIde.Dialogs.ControlDialogs
{
    /// <summary>
    ///     Interaction logic for MessageBoxCode.xaml
    /// </summary>
    public partial class MessageBoxCode
    {
        public MessageBoxCode(string title, string code)
        {
            InitializeComponent();

            lblTitle.Text = title;
            txtCode.Text = code;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}