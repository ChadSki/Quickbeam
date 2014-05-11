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

using System.Drawing;
using Quickbeam.Low.ByteArray;

namespace Quickbeam.Low.ObservableStruct
{
    /// <summary>
    /// Base class for color types, since they all look the same to WPF but need different backends.
    /// </summary>
    public abstract class ColorField : ObservableField<Color> { }

    public class ColorByteField : ColorField
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new ColorByteField { ByteArray = byteArray, Offset = offset };
        }

        public override Color Value
        {
            get
            {
                return Color.FromArgb(
                    ByteArray.ReadUInt8(Offset),
                    ByteArray.ReadUInt8(Offset + 1),
                    ByteArray.ReadUInt8(Offset + 2),
                    ByteArray.ReadUInt8(Offset + 3));
            }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteUInt8(Offset, value.A);
                ByteArray.WriteUInt8(Offset + 1, value.R);
                ByteArray.WriteUInt8(Offset + 2, value.G);
                ByteArray.WriteUInt8(Offset + 3, value.B);
                OnPropertyChanged("Value");
            }
        }
    }

    public class ColorRgbField : ColorField
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new ColorRgbField { ByteArray = byteArray, Offset = offset };
        }

        public override Color Value
        {
            get
            {
                return Color.FromArgb(
                    0,
                    (int)ByteArray.ReadFloat32(Offset),
                    (int)ByteArray.ReadFloat32(Offset + 4),
                    (int)ByteArray.ReadFloat32(Offset + 8));
            }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteFloat32(Offset, value.R);
                ByteArray.WriteFloat32(Offset + 4, value.G);
                ByteArray.WriteFloat32(Offset + 8, value.B);
                OnPropertyChanged("Value");
            }
        }
    }

    public class ColorArgbField : ColorField
    {
        public static ObservableField Create(IByteArray byteArray, int offset)
        {
            return new ColorArgbField { ByteArray = byteArray, Offset = offset };
        }

        public override Color Value
        {
            get
            {
                return Color.FromArgb(
                    (int)ByteArray.ReadFloat32(Offset),
                    (int)ByteArray.ReadFloat32(Offset + 4),
                    (int)ByteArray.ReadFloat32(Offset + 8),
                    (int)ByteArray.ReadFloat32(Offset + 12));
            }
            set
            {
                if (Value.Equals(value)) return;
                ByteArray.WriteFloat32(Offset, value.A);
                ByteArray.WriteFloat32(Offset + 4, value.R);
                ByteArray.WriteFloat32(Offset + 8, value.G);
                ByteArray.WriteFloat32(Offset + 12, value.B);
                OnPropertyChanged("Value");
            }
        }
    }
}
