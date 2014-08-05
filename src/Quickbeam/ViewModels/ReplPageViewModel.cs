using Quickbeam.Models;

namespace Quickbeam.ViewModels
{
	public class ReplPageViewModel : Base
	{
		public ReplPageViewModel()
		{
			App.Storage.HomeWindowViewModel.UpdateStatus("Welcome");
		}

	    public string HaloExePath
	    {
	        get { return App.Storage.Settings.HaloExePath; }
            set { App.Storage.Settings.HaloExePath = value; }
	    }

	    public void OpenFile(HomeViewModel.Type type)
		{
			switch (type)
			{
				case HomeViewModel.Type.BlamCache:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.BlamCache));
					break;

				case HomeViewModel.Type.MapInfo:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.MapInfo));
					break;

				case HomeViewModel.Type.MapImage:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.MapImage));
					break;

				case HomeViewModel.Type.Campaign:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.Campaign));
					break;

				case HomeViewModel.Type.Patch:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.Patch));
					break;

				case HomeViewModel.Type.Other:
					App.Storage.HomeWindowViewModel.ValidateFile(App.Storage.HomeWindowViewModel.FindFile(HomeViewModel.Type.Other));
					break;
			}
		}
	}
}
