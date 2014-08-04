using System.Collections.Generic;
using System.Windows;
using Quickbeam.Dialogs.Controls;
using Quickbeam.Views.Cache.TagEditorComponents.Data;
using Quickbeam.ViewModels;
using Quickbeam.ViewModels.Dialog;

namespace Quickbeam.Dialogs
{
	public static class MetroViewValueAs
	{
		public static void Show(CachePageViewModel cachePageViewModel, uint cacheOffsetOriginal,
			IList<TagDataField> tagDataFieldList)
		{
			var valueAs = new MetroViewValueAsWindow(new ViewValueAsViewModel(cachePageViewModel, cacheOffsetOriginal, 
				tagDataFieldList))
			{
				Owner = App.Storage.HomeWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			valueAs.Show();
		}
	}
}
