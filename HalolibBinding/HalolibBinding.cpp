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

// This is the main DLL file.

#include "stdafx.hpp"
#include "HalolibBinding.hpp"

namespace HalolibBinding {

    int callback(BoundPyObject* slf, char* name)
    {
        slf->OnPropertyChanged(name);
        return 0;
    }

    BoundPyObject::BoundPyObject(PyObject* po)
        : _po(po)
    {
        PyObject* result0 = PyObject_CallMethod(po, "register_callback", "II", // (uint, uint)
            reinterpret_cast<UINT32>(&callback),
            reinterpret_cast<UINT32>(this));
    }

    void BoundPyObject::OnPropertyChanged(char* name)
    {
        std::cout << name << " changed! (C++ OnPropertyChanged)" << std::endl;
    }

}
