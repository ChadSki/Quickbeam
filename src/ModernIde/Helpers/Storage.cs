using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace ModernIde.Helpers
{
    public class Storage : INotifyPropertyChanged
    {
        private Settings _assemblySettings = new Settings();

        #region Helpers

        public void Load()
        {
            #region Settings

            // Get File Path
            string jsonString = null;
            if (File.Exists("AssemblySettings.ason"))
                jsonString = File.ReadAllText("AssemblySettings.ason");

            try
            {
                if (jsonString == null)
                    _assemblySettings = new Settings();
                else
                    _assemblySettings = JsonConvert.DeserializeObject<Settings>(jsonString) ?? new Settings();
            }
            catch (JsonSerializationException)
            {
                _assemblySettings = new Settings();
            }
            _assemblySettings.Loaded = true;

            // Update Accent
            _assemblySettings.UpdateAssemblyAccent();

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

        public Settings AssemblySettings
        {
            get { return _assemblySettings; }
            set
            {
                // Set Data
                SetField(ref _assemblySettings, value, "AssemblySettings");

                // Write Changes
                string jsonData = JsonConvert.SerializeObject(value);

                // Get File Path
                File.WriteAllText("AssemblySettings.ason", jsonData);

                // Update Accent
                _assemblySettings.UpdateAssemblyAccent();

                // Update File Defaults
                FileDefaults.UpdateFileDefaults();
            }
        }
    }
}