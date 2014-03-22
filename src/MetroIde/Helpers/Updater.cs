using System;
using MetroIde.Helpers.Net;
using MetroIde.Dialogs;

namespace MetroIde.Helpers
{
	public class Updater
	{
		public static void BeginUpdateProcess()
		{
			// Grab JSON Update package from the server
			UpdateInfo info = Updates.GetUpdateInfo();

			// If the request failed, tell the user to gtfo
			if (info == null || !info.Successful)
			{
				App.MetroIdeStorage.MetroIdeSettings.HomeWindow.Dispatcher.Invoke(new Action(
					() =>
						MetroMessageBox.Show("Update Check Failed",
							"MetroIde is unable to check for updates at this time. Sorry :(")));
				return;
			}

			App.MetroIdeStorage.MetroIdeSettings.HomeWindow.Dispatcher.Invoke(
				new Action(() => MetroUpdateDialog.Show(info, UpdateAvailable(info))));
		}

		public static bool UpdateAvailable(UpdateInfo info)
		{
			if (info == null || !info.Successful)
				return false;

			var serverVersion = info.LatestVersion;
			var currentVersion = VariousFunctions.GetApplicationVersion();

			return (serverVersion.CompareTo(currentVersion) > 0);
        }
    }
}