using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace MetroIde.Helpers
{
    public class Storage : INotifyPropertyChanged
    {
        private Settings _metroIdeSettings = new Settings();

        #region Helpers

        public void Load()
        {
            #region Settings

            // Get File Path
            string jsonString = null;
            if (File.Exists("MetroIdeSettings.json"))
                jsonString = File.ReadAllText("MetroIdeSettings.json");

            try
            {
                if (jsonString == null)
                    _metroIdeSettings = new Settings();
                else
                    _metroIdeSettings = JsonConvert.DeserializeObject<Settings>(jsonString) ?? new Settings();
            }
            catch (JsonSerializationException)
            {
                _metroIdeSettings = new Settings();
            }
            _metroIdeSettings.Loaded = true;

            // Update Accent
            _metroIdeSettings.UpdateAssemblyAccent();

            #endregion
        }

        #endregion

        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public Storage()
        {
            Load();
        }

        public Settings MetroIdeSettings
        {
            get { return _metroIdeSettings; }
            set
            {
                // Set Data
                SetField(ref _metroIdeSettings, value, "AssemblySettings");

                // Write Changes
                string jsonData = JsonConvert.SerializeObject(value);

                // Get File Path
                File.WriteAllText("MetroIdeSettings.json", jsonData);

                // Update Accent
                _metroIdeSettings.UpdateAssemblyAccent();

                // Update File Defaults
                FileDefaults.UpdateFileDefaults();
            }
        }
    }
}