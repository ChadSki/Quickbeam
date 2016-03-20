#include "stdafx.h"
#include "ObservablePyObject.h"

namespace PythonBinding
{
#pragma managed(push, off)

    ObservablePyObject::ObservablePyObject(PyObject* po)
        : pyobj(po)
    {
        PyObject* result0 = PyObject_CallMethod(po, "register_callback", "KK", // (uint64, uint64)
            reinterpret_cast<UINT64>(&callback_thunk),
            reinterpret_cast<UINT64>(this));
    }

    /// Triggered whenever a property of the bound PyObject is updated.
    void ObservablePyObject::OnPropertyChanged()
    {
        //std::cout << "changed! (C++ OnPropertyChanged)" << std::endl;
        // TODO run event handlers too
    }

    int callback_thunk(ObservablePyObject* slf)
    {
        slf->OnPropertyChanged();
        return 0;
    }

#pragma managed(pop)
}
