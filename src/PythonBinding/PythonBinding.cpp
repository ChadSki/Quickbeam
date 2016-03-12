// This is the main DLL file.

#include "stdafx.h"

#include "PythonBinding.h"

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices;

namespace PythonBinding {

    public class ObservablePyObject
    {
    public:
        ObservablePyObject(PyObject* po)
            : _po(po)
        {
            PyObject* result0 = PyObject_CallMethod(po, "register_callback", "KK", // (uint64, uint64)
                reinterpret_cast<UINT64>(&callback_thunk),
                reinterpret_cast<UINT64>(this));
        }

        /// Triggered whenever a property of the bound PyObject is updated.
        void OnPropertyChanged()
        {
            std::cout << "changed! (C++ OnPropertyChanged)" << std::endl;
            // TODO run event handlers too
        }

        PyObject* _po;
    };

    int callback_thunk(ObservablePyObject* slf)
    {
        slf->OnPropertyChanged();
        return 0;
    }

    FieldProxy::FieldProxy() {}

    HaloStructProxy::HaloStructProxy(PyObject* halostruct)
    {
        this->halostruct = new ObservablePyObject(halostruct);
        this->_fields = gcnew FieldGroup();

        // type: Dict[str, Union[BasicField, HaloField]]
        auto fieldsDict = PyObject_GetAttrString(this->halostruct->_po, "fields");
        // type: Sequence[Tuple[str, Union[BasicField, HaloField]]]
        auto fieldsItems = PyMapping_Items(fieldsDict);
        auto fieldsIter = PyObject_GetIter(fieldsItems);
        if (fieldsIter == nullptr) {
            throw gcnew NullReferenceException(
                "Could not iterate over the struct's fields.");
        }

        // Read all the fields into our collection
        PyObject* item;
        while (item = PyIter_Next(fieldsIter)) {
            auto fieldName = PySequence_GetItem(item, 0);
            auto fieldItself = PySequence_GetItem(item, 1);

            PyObject_Print(fieldName, stdout, Py_PRINT_RAW);
            std::cout << " ";
            PyObject_Print(fieldItself, stdout, Py_PRINT_RAW);
            std::cout << std::endl;
        }
    }

    String^ HaloStructProxy::ToString()
    {
        return gcnew String("TODO");
    }

    HaloTagProxy::HaloTagProxy(PyObject* halotag)
    {
        this->halotag = halotag;
        this->_data = gcnew HaloStructProxy(PyObject_GetAttrString(this->halotag, "data"));
    }

    String^ HaloTagProxy::ToString()
    {
        return gcnew String("TODO");
    }

    HaloMapProxy::HaloMapProxy(PyObject* map)
    {
        this->halomap = map;
        this->_tags = gcnew List<PythonBinding::HaloTagProxy^>();
    }

    HaloTagProxy^ HaloMapProxy::getGhost()
    {
        auto tag_fn = PyObject_GetAttrString(this->halomap, "tag");
        auto args = PyTuple_Pack(2,
            PyUnicode_FromString("vehi"),
            PyUnicode_FromString("ghost"));
        auto ghost = PyObject_CallObject(tag_fn, args);
        return gcnew HaloTagProxy(ghost);
    }

    String^ HaloMapProxy::ToString()
    {
        return gcnew String("TODO");
    }


    PythonInterpreter::PythonInterpreter()
    {
        Py_Initialize();
        PyRun_SimpleString(
            "import sys\n"
            "sys.stdout = open('CONOUT$', 'wt')\n"  // Fix console output
            "import halolib\n");

        PyObject* sys_mod_dict = PyImport_GetModuleDict();
        PyObject* main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
        this->halolib = PyObject_GetAttrString(main_mod, "halolib");
        this->_maps = gcnew List<HaloMapProxy^>();
    }

    void PythonInterpreter::OpenMap(HaloMemory whichExe)
    {
        auto halolib_dict = PyModule_GetDict(halolib);
        auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
        PyObject* map_constructor{};
        switch (whichExe)
        {
        case HaloMemory::PC:
            std::cout << "PC" << std::endl;
            map_constructor = PyObject_GetAttrString(halomap_class, "from_hpc");
            break;

        case HaloMemory::CE:
            std::cout << "CE" << std::endl;
            map_constructor = PyObject_GetAttrString(halomap_class, "from_hce");
            break;
        }
        std::cout << "map_constructor " << map_constructor << std::endl;
        auto map = PyObject_CallObject(map_constructor, nullptr);
        std::cout << "map " << map << std::endl;
        Maps->Add(gcnew HaloMapProxy(map));
        return;
    }

    void PythonInterpreter::OpenMap(String ^ filename)
    {
        // Encode the text as UTF8, making sure the array is zero terminated
        auto encodedBytes = Encoding::UTF8->GetBytes(filename + "\0");
        // prevent GC moving the bytes around while this variable is on the stack
        pin_ptr<Byte> pinnedBytes = &encodedBytes[0];
        auto halolib_dict = PyModule_GetDict(halolib);
        auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
        auto map_constructor = PyObject_GetAttrString(halomap_class, "from_file");
        // cast pin_ptr to char*
        auto map = PyObject_CallObject(map_constructor,
            PyTuple_Pack(1, PyUnicode_FromString(reinterpret_cast<char*>(pinnedBytes))));
        Maps->Add(gcnew HaloMapProxy(map));
    }

    String^ PythonInterpreter::ToString()
    {
        return gcnew String("TODO");
    }
}
