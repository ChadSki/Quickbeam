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

#pragma once
#include "Stdafx.hpp"

using namespace System;

namespace HalolibBinding {

#pragma managed(push, off)

    /// Wraps a PyObject known to implement pnpc databinding.
    ///
    public class BoundPyObject
    {
    public:
        BoundPyObject(PyObject* po);
        BoundPyObject(const BoundPyObject &po);
        ~BoundPyObject();

        /// Triggered whenever a property of the bound PyObject
        /// is updated.
        ///
        void OnPropertyChanged(char* name);
    private:
        PyObject* _po;
    };

    /// Callback function to be passed to pnpc Python objects.
    ///
    int callback(BoundPyObject* slf, char* name);

#pragma managed(pop)

    /// Class to wrap the pseudo-main function.
    ///
    public ref class MyClass
    {
    public:
        /// Pseudo-main function for developing purposes.
        ///
        static void doThing()
        {
            Py_Initialize();
            PyRun_SimpleString(
                "import halolib\n"
                "m = halolib.load_map('beavercreek.map')\n"

                "t = m.get_tag('bipd')\n"
                "t.turn_speed *= 1.05\n"
                );

            PyObject* sys_mod_dict = PyImport_GetModuleDict();
            PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
            PyObject* halomap = PyObject_GetAttrString(main_mod, "m");

            BoundPyObject* bpo = new BoundPyObject(halomap);

            int result = PyObject_SetAttr(halomap, PyUnicode_FromString("asdf"), PyLong_FromLong(14));

            Py_Finalize();
        }
    };
}


