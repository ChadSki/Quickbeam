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
        private bool _defaultAmp;
        private bool _defaultBlf;
        private bool _defaultCif;
        private bool _defaultMap;
        private bool _defaultMif;
        private Home _homeWindow;
        private bool _startpageHideOnLaunch;
        private bool _startpageShowOnLoad = true;
        private bool _startpageShowRecentsBlf = true;
        private bool _startpageShowRecentsCampaign = true;
        private bool _startpageShowRecentsMap = true;
        private bool _startpageShowRecentsMapInfo = true;
        private string _xsdPath = "";

        #region Enums

        public enum Accents
        {
            Blue,
            Purple,
            Orange,
            Green
        }

        public enum MapInfoDockSide
        {
            Left,
            Right
        }

        public enum RecentFileType
        {
            Cache,
            MapInfo,
            Blf,
            Campaign
        }

        public enum TagSort
        {
            TagClass,
            ObjectHierarchy,
            PathHierarchy
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
            File.WriteAllText("MetroIdeSettings.json", jsonData);

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

        public bool StartpageShowOnLoad
        {
            get { return _startpageShowOnLoad; }
            set { SetField(ref _startpageShowOnLoad, value, "StartpageShowOnLoad"); }
        }

        public bool StartpageHideOnLaunch
        {
            get { return _startpageHideOnLaunch; }
            set { SetField(ref _startpageHideOnLaunch, value, "StartpageHideOnLaunch"); }
        }

        public bool StartpageShowRecentsMap
        {
            get { return _startpageShowRecentsMap; }
            set { SetField(ref _startpageShowRecentsMap, value, "StartpageShowRecentsMap"); }
        }

        public bool StartpageShowRecentsBlf
        {
            get { return _startpageShowRecentsBlf; }
            set { SetField(ref _startpageShowRecentsBlf, value, "StartpageShowRecentsBlf"); }
        }

        public bool StartpageShowRecentsMapInfo
        {
            get { return _startpageShowRecentsMapInfo; }
            set { SetField(ref _startpageShowRecentsMapInfo, value, "StartpageShowRecentsMapInfo"); }
        }

        public bool StartpageShowRecentsCampaign
        {
            get { return _startpageShowRecentsCampaign; }
            set { SetField(ref _startpageShowRecentsCampaign, value, "StartpageShowRecentsCampaign"); }
        }

        public string XsdPath
        {
            get { return _xsdPath; }
            set { SetField(ref _xsdPath, value, "XsdPath"); }
        }

        public bool DefaultMap
        {
            get { return _defaultMap; }
            set
            {
                SetField(ref _defaultMap, value, "DefaultMap");
                if (Loaded)
                    FileDefaults.UpdateFileDefaults();
            }
        }

        public bool DefaultBlf
        {
            get { return _defaultBlf; }
            set
            {
                SetField(ref _defaultBlf, value, "DefaultBlf");
                if (Loaded)
                    FileDefaults.UpdateFileDefaults();
            }
        }

        public bool DefaultMif
        {
            get { return _defaultMif; }
            set
            {
                SetField(ref _defaultMif, value, "DefaultMif");
                if (Loaded)
                    FileDefaults.UpdateFileDefaults();
            }
        }

        public bool DefaultCif
        {
            get { return _defaultCif; }
            set
            {
                SetField(ref _defaultCif, value, "DefaultCif");
                if (Loaded)
                    FileDefaults.UpdateFileDefaults();
            }
        }

        public bool DefaultAmp
        {
            get { return _defaultAmp; }
            set
            {
                SetField(ref _defaultAmp, value, "DefaultAmp");
                if (Loaded)
                    FileDefaults.UpdateFileDefaults();
            }
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
                    Source = new Uri("/MetroIde;component/Themes/" + theme + ".xaml", UriKind.Relative)
                });
            }
            catch
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("/MetroIde;component/Themes/Blue.xaml", UriKind.Relative)
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