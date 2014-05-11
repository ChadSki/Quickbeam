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

using System;
using System.Linq;
using Quickbeam.Low.ByteArray;

namespace Quickbeam.Low.ObservableStruct
{
    public class AsciiField : ObservableField<string>
    {
        public int Length { get; protected set; }
        public bool Reverse { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, int length, bool reverse)
        {
            return new AsciiField { ByteArray = byteArray, Offset = offset, Length = length, Reverse = reverse };
        }

        public override string Value
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
    public class AsciizField : ObservableField<string>
    {
        public int MaxLength { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, int maxLength)
        {
            return new AsciizField { ByteArray = byteArray, Offset = offset, MaxLength = maxLength };
        }

        public override string Value
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public class RawDataField : ObservableField<string>
    {
        public int Length { get; protected set; }
        public bool Reverse { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, int length, bool reverse)
        {
            return new RawDataField { ByteArray = byteArray, Offset = offset, Length = length, Reverse = reverse };
        }

        public override string Value
        {
            get { return BitConverter.ToString(ByteArray.ReadBytes(Offset, Length)); }
            set
            {
                //TODO untested code -- steps by 3 because of dashes
                var bytes = Enumerable.Range(0, value.Length)
                                      .Where(x => x % 3 == 0)
                                      .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                                      .ToArray();

                if (bytes.Length <= Length)
                    ByteArray.WriteBytes(Offset, bytes);
                else
                    throw new ArgumentException("Value too long!");
            }
        }
    }
}