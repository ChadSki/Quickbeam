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

using System.Collections.Generic;
using Quickbeam.Low.ByteArray;

namespace Quickbeam.Low.ObservableStruct
{
    public class Enum8Field : UInt8Field
    {
        public IDictionary<byte, string> Options { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, IDictionary<byte, string> options)
        {
            return new Enum8Field { ByteArray = byteArray, Offset = offset, Options = options };
        }
    }

    public class Enum16Field : UInt16Field
    {
        public IDictionary<ushort, string> Options { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, IDictionary<ushort, string> options)
        {
            return new Enum16Field { ByteArray = byteArray, Offset = offset, Options = options };
        }
    }

    public class Enum32Field : UInt32Field
    {
        public IDictionary<uint, string> Options { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, IDictionary<uint, string> options)
        {
            return new Enum32Field { ByteArray = byteArray, Offset = offset, Options = options };
        }
    }
}
