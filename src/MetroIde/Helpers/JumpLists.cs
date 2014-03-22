using System.Windows;
using System.Windows.Shell;

namespace MetroIde.Helpers
{
    public class JumpLists
    {
        public static void UpdateJumplists()
        {
            var jump = new JumpList();
            JumpList.SetJumpList(Application.Current, jump);

            if (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i > App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Count - 1)
                        break;

                    var task = new JumpTask();
                    int iconIndex = -200;
                    switch (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FileType)
                    {
                        case Settings.RecentFileType.Blf:
                            iconIndex = -200;
                            break;
                        case Settings.RecentFileType.Cache:
                            iconIndex = -201;
                            break;
                        case Settings.RecentFileType.MapInfo:
                            iconIndex = -202;
                            break;
                    }

                    task.ApplicationPath = VariousFunctions.GetApplicationAssemblyLocation();
                    task.Arguments = string.Format("assembly://open \"{0}\"",
                        App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FilePath);
                    task.WorkingDirectory = VariousFunctions.GetApplicationLocation();

                    task.IconResourcePath = VariousFunctions.GetApplicationLocation() + "AssemblyIconLibrary.dll";
                    task.IconResourceIndex = iconIndex;

                    task.CustomCategory = "Recent";
                    task.Title = App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FileName + " - " +
                                 App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FileGame;
                    task.Description = string.Format("Open {0} in Assembly. ({1})",
                        App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FileName,
                        App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents[i].FilePath);

                    jump.JumpItems.Add(task);
                }
            }

            // Show Recent and Frequent categories :D
            jump.ShowFrequentCategory = false;
            jump.ShowRecentCategory = false;

            // Send to the Windows Shell
            jump.Apply();
        }
    }
}