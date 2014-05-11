using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using MetroIde.Dialogs;
using Newtonsoft.Json;

namespace MetroIde.Helpers
{
    public class Settings : INotifyPropertyChanged
    {
        private Accents _applicationAccent = Accents.Orange;
        private ObservableCollection<RecentFileEntry> _applicationRecents = new ObservableCollection<RecentFileEntry>();
        private double _applicationSizeHeight = 600;
        private bool _applicationSizeMaximize;
        private double _applicationSizeWidth = 1100;
        private bool _applicationUpdateOnStartup = true;
        private Home _homeWindow;
        private string _haloExePath = "";
        private bool _autoDetectFullscreenResolution = true;
        private int _haloWindowedWidth = 800;
        private int _haloWindowedHeight = 600;
        private int _haloDockedWidth = 800;
        private int _haloDockedHeight = 600;

        #region Enums

        public enum Accents
        {
            Blue,
            Orange,
            Green
        }

        public enum RecentFileType
        {
            Cache,
            MapInfo,
            Blf,
            Campaign
        }

        #endregion

        public class RecentFileEntry
        {
            public string FileName { get; set; }
            public RecentFileType FileType { get; set; }
            public string FileGame { get; set; }
            public string FilePath { get; set; }
        }

        public Settings()
        {
            ApplicationRecents.CollectionChanged +=
                (sender, args) => SetField(ref _applicationRecents, sender as ObservableCollection<RecentFileEntry>,
                    "ApplicationRecents", true);
        }

        [JsonIgnore]
        public bool Loaded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName, bool overrideChecks = false)
        {
            if (!overrideChecks)
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return false;

            field = value;
            OnPropertyChanged(propertyName);

            if (!Loaded)
                return true;

            // Write Changes
            string jsonData = JsonConvert.SerializeObject(this);
            File.WriteAllText("QuickbeamSettings.json", jsonData);

            return true;
        }

        /// <summary>
        ///     The Accent colour the user has selected. Defaults to Assembly Blue.
        /// </summary>
        public Accents ApplicationAccent
        {
            get { return _applicationAccent; }
            set
            {
                SetField(ref _applicationAccent, value, "ApplicationAccent");
                UpdateAssemblyAccent();
            }
        }

        /// <summary>
        ///     Wether Assebembly checks for updates at the startup. Defaults to True.
        /// </summary>
        public bool ApplicationUpdateOnStartup
        {
            get { return _applicationUpdateOnStartup; }
            set { SetField(ref _applicationUpdateOnStartup, value, "ApplicationUpdateOnStartup"); }
        }

        public bool AutoDetectFullscreenResolution
        {
            get { return _autoDetectFullscreenResolution; }
            set { SetField(ref _autoDetectFullscreenResolution, value, "AutoDetectFullscreenResolution"); }
        }

        /// <summary>
        ///     A list of Assembly's recently opened files.
        /// </summary>
        public ObservableCollection<RecentFileEntry> ApplicationRecents
        {
            get { return _applicationRecents; }
            set { SetField(ref _applicationRecents, value, "ApplicationRecents"); }
        }

        public double ApplicationSizeWidth
        {
            get { return _applicationSizeWidth; }
            set { SetField(ref _applicationSizeWidth, value, "ApplicationSizeWidth"); }
        }

        public double ApplicationSizeHeight
        {
            get { return _applicationSizeHeight; }
            set { SetField(ref _applicationSizeHeight, value, "ApplicationSizeHeight"); }
        }

        public bool ApplicationSizeMaximize
        {
            get { return _applicationSizeMaximize; }
            set { SetField(ref _applicationSizeMaximize, value, "ApplicationSizeMaximize"); }
        }

        public string HaloExePath
        {
            get { return _haloExePath; }
            set { SetField(ref _haloExePath, value, "HaloExePath"); }
        }

        public int HaloWindowedWidth
        {
            get { return _haloWindowedWidth; }
            set { SetField(ref _haloWindowedWidth, value, "HaloWindowedWidth"); }
        }

        public int HaloWindowedHeight
        {
            get { return _haloWindowedHeight; }
            set { SetField(ref _haloWindowedHeight, value, "HaloWindowedHeight"); }
        }

        public int HaloDockedWidth
        {
            get { return _haloDockedWidth; }
            set { SetField(ref _haloDockedWidth, value, "HaloDockedWidth"); }
        }

        public int HaloDockedHeight
        {
            get { return _haloDockedHeight; }
            set { SetField(ref _haloDockedHeight, value, "HaloDockedHeight"); }
        }

        [JsonIgnore]
        public Home HomeWindow
        {
            get { return _homeWindow; }
            set { SetField(ref _homeWindow, value, "HomeWindow"); }
        }

        public void UpdateAssemblyAccent()
        {
            string theme =
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                    Enum.Parse(typeof (Accents), ApplicationAccent.ToString()).ToString());
            try
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("/Quickbeam;component/Themes/" + theme + ".xaml", UriKind.Relative)
                });
            }
            catch
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("/Quickbeam;component/Themes/Blue.xaml", UriKind.Relative)
                });
            }
        }
    }

    public class TempStorage
    {
        public static MetroMessageBox.MessageBoxResult MessageBoxButtonStorage;

        public static KeyValuePair<string, int> TagBookmarkSaver;
    }

    public class RecentFiles
    {
        public static void AddNewEntry(string filename, string filepath, string game, Settings.RecentFileType type)
        {
            Settings.RecentFileEntry alreadyExistsEntry = null;

            if (App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents == null)
                App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents =
                    new ObservableCollection<Settings.RecentFileEntry>();

            foreach (
                Settings.RecentFileEntry entry in
                    App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Where(
                        entry => entry.FileName == filename && entry.FilePath == filepath && entry.FileGame == game))
                alreadyExistsEntry = entry;

            if (alreadyExistsEntry == null)
            {
                // Add New Entry
                var newEntry = new Settings.RecentFileEntry
                {
                    FileGame = game,
                    FileName = filename,
                    FilePath = filepath,
                    FileType = type
                };
                App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Insert(0, newEntry);
            }
            else
            {
                // Move existing Entry
                App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Remove(alreadyExistsEntry);
                App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Insert(0, alreadyExistsEntry);
            }

            JumpLists.UpdateJumplists();
        }

        public static void RemoveEntry(Settings.RecentFileEntry entry)
        {
            App.MetroIdeStorage.MetroIdeSettings.ApplicationRecents.Remove(entry);
        }
    }
}