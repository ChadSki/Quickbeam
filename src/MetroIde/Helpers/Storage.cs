using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace MetroIde.Helpers
{
    public class Storage : INotifyPropertyChanged
    {
        private Settings _modernIdeSettings = new Settings();

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
                    _modernIdeSettings = new Settings();
                else
                    _modernIdeSettings = JsonConvert.DeserializeObject<Settings>(jsonString) ?? new Settings();
            }
            catch (JsonSerializationException)
            {
                _modernIdeSettings = new Settings();
            }
            _modernIdeSettings.Loaded = true;

            // Update Accent
            _modernIdeSettings.UpdateAssemblyAccent();

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
            get { return _modernIdeSettings; }
            set
            {
                // Set Data
                SetField(ref _modernIdeSettings, value, "AssemblySettings");

                // Write Changes
                string jsonData = JsonConvert.SerializeObject(value);

                // Get File Path
                File.WriteAllText("AssemblySettings.ason", jsonData);

                // Update Accent
                _modernIdeSettings.UpdateAssemblyAccent();

                // Update File Defaults
                FileDefaults.UpdateFileDefaults();
            }
        }
    }
}