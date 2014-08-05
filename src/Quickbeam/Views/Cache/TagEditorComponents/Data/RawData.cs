

namespace Quickbeam.Views.Cache.TagEditorComponents.Data
{
	/// <summary>
	///     Base class for raw data.
	/// </summary>
	public class RawData : ValueField
	{
		private uint _dataAddress;
		private string _format;
		private int _length;

		public RawData(string name, uint offset, uint address, string value, int length, uint pluginLine)
			: base(name, offset, address, pluginLine)
		{
			_length = length;
		}

		public RawData(string name, uint offset, string format, uint address, string value, int length, uint pluginLine)
			: base(name, offset, address, pluginLine)
		{
			_length = length;
			_format = format;
		}

		public string Kind
		{
			get { return "byte array"; }
		}

		public string Format
		{
			get { return _format; }
			set
			{
				_format = value;
				OnPropertyChanged("Format");
			}
		}

		public uint DataAddress
		{
			get { return _dataAddress; }
			set
			{
				_dataAddress = value;
				OnPropertyChanged("DataAddress");
				OnPropertyChanged("DataAddressHex");
			}
		}

		public string DataAddressHex
		{
			get { return "0x" + DataAddress.ToString("X"); }
		}

		public int Length
		{
			get { return _length; }
			set
			{
				_length = value;
				OnPropertyChanged("Length");
				OnPropertyChanged("MaxLength");
			}
		}

		public int MaxLength
		{
			get { return _format == "bytes" ? _length*2 : _length; }
		}

		public override void Accept(ITagDataFieldVisitor visitor)
		{
			visitor.VisitRawData(this);
		}

		public override TagDataField CloneValue()
		{
			return new RawData(Name, Offset, FieldAddress, "", _length, PluginLine);
		}
	}
}