using System;
using System.Collections.Generic;
using System.ComponentModel;
using MetroIde.Annotations;

namespace MetroIde.Helpers.Lobby
{
    public class HaloServer : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public HaloServer(string address)
        {
            var foo = address.Split(':');
            IpAddress = foo[0];
            Port = Int32.Parse(foo[1]);
        }

        #region Properties
        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress;}
            set { SetField(ref _ipAddress, value, "IpAddress"); }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set { SetField(ref _port, value, "Port"); }
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value, "Name"); }
        }
        #endregion

    }
}
