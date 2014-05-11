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
using System.Collections.Generic;
using Quickbeam.Low.ByteArray;

namespace Quickbeam.Low.ObservableStruct
{
    public class ReflexiveField : ObservableField<IEnumerable<ObservableStruct>>
    {
        public dynamic HaloMap { get; protected set; }
        public dynamic ReflexiveClass { get; protected set; }
        public IByteArrayBuilder ByteArrayBuilder { get; protected set; }

        public static ObservableField Create(IByteArray byteArray, int offset, dynamic halomap, object reflexiveClass)
        {
            return new ReflexiveField { ByteArray = byteArray, Offset = offset, HaloMap = halomap, ReflexiveClass = reflexiveClass };
        }

        public override IEnumerable<ObservableStruct> Value
        {
            get
            {
                uint count = ByteArray.ReadUInt32(0);
                uint rawOffset = ByteArray.ReadUInt32(4);

                uint startOffset = rawOffset - HaloMap.map_magic;

                for(uint ii = 0; ii < count; ii++ )
                {
                    yield return ReflexiveClass(
                                    ByteArrayBuilder.Create(
                                        startOffset + ReflexiveClass.struct_size*ii,
                                        ReflexiveClass.struct_size),
                                    HaloMap);
                }
            }
            set { throw new NotImplementedException("Cannot assign to reflexives (yet)."); }
        }
    }
}