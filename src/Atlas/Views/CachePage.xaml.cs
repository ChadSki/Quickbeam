using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Atlas.Dialogs;
using Atlas.Helpers.Tags;
using Atlas.Metro.Controls.Custom;
using Atlas.Models;
using Atlas.ViewModels;
using Atlas.Views.Cache;
using Atlas.Views.Cache.Dialogs;
using Atlas.Views.Cache.TagEditorComponents.Data;
using Blamite.Blam.Scripting;

namespace Atlas.Views
{
	/// <summary>
	/// Interaction logic for CachePage.xaml
	/// </summary>
	public partial class CachePage : IAssemblyPage
	{
		public readonly CachePageViewModel ViewModel;

		public CachePage(string cacheLocation)
		{
			InitializeComponent();

			ViewModel = new CachePageViewModel(this);
			DataContext = ViewModel;
			ViewModel.LoadCache(cacheLocation);

			// woo
			AddHandler(MetroClosableTabItem.CloseTabEvent, new RoutedEventHandler(CloseTab));
		}

		public bool Close()
		{
			var letsContinue = ViewModel.Editors.All(editor => editor.Close());

			if (!letsContinue)
				return false;

			// r u shure
			var result = MetroMessageBox.Show("Are you sure?",
				"Are you sure you want to close this cache? Unsaved changes will be lost.",
				new List<MetroMessageBox.MessageBoxButton>
				{
					MetroMessageBox.MessageBoxButton.Yes,
					MetroMessageBox.MessageBoxButton.No,
					MetroMessageBox.MessageBoxButton.Cancel
				});

			var close = result == MetroMessageBox.MessageBoxButton.Yes;
			if (close) App.Storage.HomeWindowViewModel.RecentEditors.Clear();
			return close;
		}

		private void OpenTagContextMenu_OnClick(object sender, RoutedEventArgs e)
		{
			var tagHierarchyNode = NodeFromContextMenu(sender);
			if (tagHierarchyNode == null) return;

			ViewModel.LoadTagEditor(tagHierarchyNode);
		}
		private void TagTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var treeView = sender as TreeView;
			if (treeView == null) return;
			var node = treeView.SelectedItem as TagHierarchyNode;
			if (node == null) return;
			if (node.IsFolder) return;
			ViewModel.LoadTagEditor(node);

		}
		private void JumpToTagCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var tag = e.Parameter as TagHierarchyNode;
			if (tag != null) ViewModel.LoadTagEditor(tag);
		}
		private void ExtractTagContextMenu_OnClick(object sender, RoutedEventArgs e)
		{
			var tagHierarchyNode = NodeFromContextMenu(sender);
			if (tagHierarchyNode == null) return;

			MetroTagExtractor.Show(ViewModel.CacheFile, ViewModel.EngineDescription, ViewModel.MapStreamManager, tagHierarchyNode);
		}
		private void RenameNodeContextMenu_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ScriptButton_OnClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null) return;

			ViewModel.LoadScriptEditor(button.DataContext as IScriptFile);
		}


		#region Helpers

		public TagHierarchyNode NodeFromContextMenu(object sender)
		{
			var menuItem = sender as MenuItem;
			if (menuItem == null) return null;

			var contextMenu = menuItem.Parent as ContextMenu;
			if (contextMenu == null) return null;

			return contextMenu.DataContext as TagHierarchyNode;
		}

		#endregion

		#region Script Editor Toolbar Buttons

		private void ScriptEditorExportButton_OnClick(object sender, RoutedEventArgs e)
		{
			var editor = ViewModel.SelectedEditor as ScriptEditor;
			if (editor == null) return;
			editor.ViewModel.ExportScript();
		}

		#endregion


		private void CloseTab(object source, RoutedEventArgs args)
		{
			var editor = ((MetroClosableTabItem)args.OriginalSource).Content as ICacheEditor;
			if (editor == null) return;
			if (editor.Close())
				ViewModel.Editors.Remove(editor);
		}
	}
}
