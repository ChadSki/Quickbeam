using Newtonsoft.Json;
using Quickbeam.Interfaces;
using Quickbeam.ViewModels;
using Quickbeam.Windows;
using System.Diagnostics;
using System.IO;

namespace Quickbeam.Helpers
{
    public class Storage : Base
    {
        public const string StoragePath = @".\";
        public const string SettingsPath = @"Settings.json";

        public Storage()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            ParseSettings();

            // Set up file Watching
            if (!File.Exists(SettingsPath))
                return;

            var fileWatcher = new FileSystemWatcher(StoragePath)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.LastAccess
            };
            fileWatcher.Changed += (sender, args) =>
            {
                if (Settings != null && Settings.Changed)
                {
                    Settings.Changed = false;
                    return;
                }
                ParseSettings();
            };
            fileWatcher.EnableRaisingEvents = true;
        }

        private void ParseSettings()
        {
            try
            {
                // Get File Path
                string jsonString = null;
                if (File.Exists(SettingsPath))
                    jsonString = File.ReadAllText(SettingsPath);

                try
                {
                    if (jsonString == null)
                        _settings = new Settings();
                    else
                        _settings = JsonConvert.DeserializeObject<Settings>(jsonString) ?? new Settings();
                }
                catch (JsonSerializationException)
                {
                    _settings = new Settings();
                }
                _settings.Loaded = true;
            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                // Set Data
                SetField(ref _settings, value);

                // Write Changes
                var jsonData = JsonConvert.SerializeObject(value);

                // Get File Path
                File.WriteAllText(SettingsPath, jsonData);
            }
        }
        private Settings _settings = new Settings();

        public IView MainPage { get; set; }
    }
}
