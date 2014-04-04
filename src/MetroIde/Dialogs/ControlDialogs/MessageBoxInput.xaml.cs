﻿using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroIde.Helpers.Native;

namespace MetroIde.Dialogs.ControlDialogs
{
    /// <summary>
    ///     Interaction logic for MessageBoxInput.xaml
    /// </summary>
    public partial class MessageBoxInput
    {
        private readonly string _regexMatch;

        public MessageBoxInput(string title, string message, string placeholder, string regexMatch, string defaultText)
        {
            InitializeComponent();
            lblTitle.Text = title;
            lblSubInfo.Text = message;
            lblPlaceholder.Text = placeholder;
            txtInput.Text = defaultText;
            _regexMatch = regexMatch;

            // Note to Alex: this is going to fuck with how you implemented the placeholder text,
            // but I think the textbox should be focused by default
            // Maybe you can make the placeholder text always show if length == 0?
            txtInput.Focus();
            txtInput.Select(txtInput.Text.Length, 0);

            //txtInput_LostFocus(txtInput, null);
        }

        public string Result { get; private set; }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            txtInput.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));

            base.OnMouseDown(e);
        }

        private void txtInput_GotFocus(object sender, RoutedEventArgs e)
        {
            lblPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void txtInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox)) return;
            var textbox = sender as TextBox;
            lblPlaceholder.Visibility = string.IsNullOrEmpty(textbox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void lblPlaceholder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: hacky thing to give textbox focus on click
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            ValidateInput();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                ValidateInput();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ValidateInput()
        {
            if (_regexMatch != null)
            {
                string data = txtInput.Text;
                Match match = Regex.Match(data, _regexMatch, RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    // TODO: aaron, make this more user friendly, huehue
                    MetroMessageBox.Show("Invalid Input", "The text you entered was not valid.");
                    return;
                }
            }

            Result = txtInput.Text;
            Close();
        }
    }
}