#pragma once

namespace CrappyCppBinding
{
#pragma managed(push, off)

    /// Wraps a PyObject known to have a property_changed Event.
    public class ObservablePyObject
    {
    public:
        ObservablePyObject(PyObject* po);

        /// Triggered whenever a property of the bound PyObject is updated.
        void OnPropertyChanged();

        PyObject* pyobj;
    };

    /// Callback function to be passed into Python.
    int callback_thunk(ObservablePyObject* slf);

#pragma managed(pop)
}
