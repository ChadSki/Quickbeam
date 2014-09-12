﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Quickbeam.Models;
using Newtonsoft.Json;

namespace Quickbeam.Helpers
{
	public class Settings : INotifyPropertyChanged
	{
		public Settings()
		{
		}

        // Halo
        private string _haloExePath;

		// UI
		private Accent _assemblyAccent = Accent.Orange;
		[JsonIgnore]
		public bool Loaded { get; set; }

		[JsonIgnore]
		public bool Changed { get; set; }


		#region User Interface

		public Accent AssemblyAccent
		{
			get { return _assemblyAccent; }
			set
			{
				SetField(ref _assemblyAccent, value);

				// set accent
				var accent =
					CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Enum.Parse(typeof (Accent), _assemblyAccent.ToString()).ToString());

				try
				{
					Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
					{
						Source = new Uri("/Quickbeam;component/Metro/Accents/" + accent + ".xaml", UriKind.Relative)
					});
				}
				catch
				{
					Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
					{
						Source = new Uri("/Quickbeam;component/Metro/Accents/Orange.xaml", UriKind.Relative)
					});
				}
			}
		}

		#endregion

        public string HaloExePath
        {
            get { return _haloExePath; }
            set { SetField(ref _haloExePath, value); }
        }

		public enum Accent
		{
			Blue,
			Green,
			Orange,
			Purple
		}

		#region Inpc

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool SetField<T>(ref T field, T value,
			[CallerMemberName] string propertyName = "", bool overrideChecks = false)
		{
			return SetFieldExplicit(ref field, value, propertyName, overrideChecks);
		}

		public bool SetFieldExplicit<T>(ref T field, T value, 
			string propertyName, bool overrideChecks)
		{
			if (!overrideChecks)
				if (EqualityComparer<T>.Default.Equals(field, value))
					return false;

			Changed = true;
			field = value;
			OnPropertyChanged(propertyName);

			if (!Loaded)
				return true;

			// Write Changes
			var jsonData = JsonConvert.SerializeObject(this);
			File.WriteAllText(Storage.SettingsPath, jsonData);

			return true;
		}

		#endregion
	}
}
