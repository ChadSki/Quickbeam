using System.Windows;
using Quickbeam.Helpers.Tags;
using Quickbeam.ViewModels.Cache.Dialog;
using Quickbeam.Views.Cache.Dialogs.Controls;
using Blamite.Blam;
using Blamite.Flexibility;
using Blamite.IO;

namespace Quickbeam.Views.Cache.Dialogs
{
	public static class MetroTagExtractor
	{
		public static void Show(ICacheFile cacheFile, EngineDescription engineDescription, IStreamManager streamManager, TagHierarchyNode tag)
		{
			var dialog = new TagExtractorWindow(new TagExtractorViewModel(cacheFile, engineDescription, streamManager, tag))
			{
				Owner = App.Storage.HomeWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			App.Storage.HomeWindowViewModel.ShowDialog();
			dialog.ShowDialog();
			App.Storage.HomeWindowViewModel.HideDialog();
		}
	}
}
