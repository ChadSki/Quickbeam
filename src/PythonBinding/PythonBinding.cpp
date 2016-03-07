// This is the main DLL file.

#include "stdafx.h"

#include "PythonBinding.h"

namespace PythonBinding {

    BoundPyObject::BoundPyObject(PyObject* po)
        : _po(po)
    {
        PyObject* result0 = PyObject_CallMethod(po, "register_callback", "KK", // (uint64, uint64)
            reinterpret_cast<UINT64>(&callback),
            reinterpret_cast<UINT64>(this));
    }

    BoundPyObject::~BoundPyObject()
    {
        // TODO
    }

    void BoundPyObject::OnPropertyChanged()
    {
        std::cout << "changed! (C++ OnPropertyChanged)" << std::endl;
    }

    int callback(BoundPyObject* slf)
    {
        slf->OnPropertyChanged();
        return 0;
    }

}
