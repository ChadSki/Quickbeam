// Copyright (c) 2013, Chad Zawistowski
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Quickbeam.Low.ByteArray;

namespace Quickbeam.Low.ObservableStruct
{
    #region Signed Integers

    public class Int8Field : ObservableField<sbyte>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Int8Field { ByteArray = byteArray, Offset = offset };
        }

        public override sbyte Value
        {
            get { return ByteArray.ReadInt8(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteInt8(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class Int16Field : ObservableField<short>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Int16Field { ByteArray = byteArray, Offset = offset };
        }

        public override short Value
        {
            get { return ByteArray.ReadInt16(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteInt16(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class Int32Field : ObservableField<int>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Int32Field { ByteArray = byteArray, Offset = offset };
        }

        public override int Value
        {
            get { return ByteArray.ReadInt32(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteInt32(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class Int64Field : ObservableField<long>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Int64Field { ByteArray = byteArray, Offset = offset };
        }

        public override long Value
        {
            get { return ByteArray.ReadInt64(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteInt64(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }

    #endregion

    #region Unsigned Integers

    public class UInt8Field : ObservableField<byte>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new UInt8Field { ByteArray = byteArray, Offset = offset };
        }

        public override byte Value
        {
            get { return ByteArray.ReadUInt8(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteUInt8(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class UInt16Field : ObservableField<ushort>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new UInt16Field { ByteArray = byteArray, Offset = offset };
        }

        public override ushort Value
        {
            get { return ByteArray.ReadUInt16(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteUInt16(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class UInt32Field : ObservableField<uint>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new UInt32Field { ByteArray = byteArray, Offset = offset };
        }

        public override uint Value
        {
            get { return ByteArray.ReadUInt32(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteUInt32(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }
    public class UInt64Field : ObservableField<ulong>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new UInt64Field { ByteArray = byteArray, Offset = offset };
        }

        public override ulong Value
        {
            get { return ByteArray.ReadUInt64(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteUInt64(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }

    #endregion

    #region Floating-Point

    public class Float32Field : ObservableField<float>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Float32Field { ByteArray = byteArray, Offset = offset };
        }

        public override float Value
        {
            get { return ByteArray.ReadFloat32(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteFloat32(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }

    public class Float64Field : ObservableField<double>
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new Float64Field { ByteArray = byteArray, Offset = offset };
        }

        public override double Value
        {
            get { return ByteArray.ReadFloat64(Offset); }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteFloat64(Offset, value);
                OnPropertyChanged("Value");
            }
        }
    }

    #endregion
}