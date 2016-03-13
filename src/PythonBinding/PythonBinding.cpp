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
            : pyobj(po)
        {
            PyObject* result0 = PyObject_CallMethod(po, "register_callback", "KK", // (uint64, uint64)
                reinterpret_cast<UINT64>(&callback_thunk),
                reinterpret_cast<UINT64>(this));
        }

        /// Triggered whenever a property of the bound PyObject is updated.
        void OnPropertyChanged()
        {
            //std::cout << "changed! (C++ OnPropertyChanged)" << std::endl;
            // TODO run event handlers too
        }

        PyObject* pyobj;
    };

    int callback_thunk(ObservablePyObject* slf)
    {
        slf->OnPropertyChanged();
        return 0;
    }

    FieldProxy::FieldProxy(PyObject* field) {}

    HaloStructProxy::HaloStructProxy(PyObject* halostruct)
    {
        this->halostruct = new ObservablePyObject(halostruct);
        this->fields = gcnew Dictionary<String^, FieldEntry^>();

        // type: Dict[str, Union[BasicField, HaloField]]
        auto fieldsDict = PyObject_GetAttrString(this->halostruct->pyobj, "fields");
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
            // Name
            auto fieldNameObj = PySequence_GetItem(item, 0);
            auto fieldNameChar = PyUnicode_AsUTF8AndSize(fieldNameObj, nullptr);
            auto fieldNameStr = gcnew String(fieldNameChar);

            //Field
            auto fieldObj = PySequence_GetItem(item, 1);
            auto entry = gcnew FieldEntry(FieldType::Int32, gcnew FieldProxy(fieldObj));
            this->Fields->Add(fieldNameStr, entry);
        }
    }

    String^ HaloStructProxy::ToString()
    {
        return String::Format("HaloStruct with {0} fields.", this->Fields->Count);
    }

    HaloTagProxy::HaloTagProxy(PyObject* halotag)
    {
        this->halotag = halotag;
        this->header = gcnew HaloStructProxy(PyObject_GetAttrString(this->halotag, "header"));
        //std::cout << "Has data: " << PyObject_HasAttrString(this->halotag, "data") << std::endl;
        //this->data = gcnew HaloStructProxy(PyObject_GetAttrString(this->halotag, "data"));
        this->noChildren = gcnew ObservableCollection<ExplorerNode^>();
    }

    String^ HaloTagProxy::ToString()
    {
        return gcnew String("HaloTag.");
    }

    HaloMapProxy::HaloMapProxy(PyObject* map)
    {
        this->halomap = map;
        this->tags = gcnew ObservableCollection<ExplorerNode^>();

        auto tags_fn = PyObject_GetAttrString(this->halomap, "tags");
        auto allTags = PyObject_CallObject(tags_fn, nullptr);
        auto tagsIter = PyObject_GetIter(allTags);
        if (tagsIter == nullptr) {
            throw gcnew NullReferenceException(
                "Could not iterate over tags.");
        }

        // Read all the tags into our collection
        PyObject* item;
        while (item = PyIter_Next(tagsIter)) {
            this->tags->Add(gcnew HaloTagProxy(item));
        }
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
        return String::Format("HaloMap with {0} tags.", this->tags->Count);
    }

    PythonInterpreter::PythonInterpreter()
    {
        Py_Initialize();
        PyRun_SimpleString(
            "import sys\n"
            //"sys.stdout = open('CONOUT$', 'wt')\n"  // Fix console output
            "import halolib\n");

        auto sys_mod_dict = PyImport_GetModuleDict();
        auto main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
        auto halolib = PyObject_GetAttrString(main_mod, "halolib");
        auto halolib_dict = PyModule_GetDict(halolib);
        this->halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
        this->maps = gcnew ObservableCollection<ExplorerNode^>();
        
    }

    void PythonInterpreter::OpenMap(HaloMemory whichExe)
    {
        PyObject* map_constructor{};
        switch (whichExe)
        {
        case HaloMemory::PC:
            map_constructor = PyObject_GetAttrString(halomap_class, "from_hpc");
            break;

        case HaloMemory::CE:
            map_constructor = PyObject_GetAttrString(halomap_class, "from_hce");
            break;
        }
        auto map = PyObject_CallObject(map_constructor, nullptr);
        this->maps->Add(gcnew HaloMapProxy(map));
    }

    void PythonInterpreter::OpenMap(String ^ filename)
    {
        // Encode the text as UTF8, making sure the array is zero terminated
        auto encodedBytes = Encoding::UTF8->GetBytes(filename + "\0");
        // prevent GC moving the bytes around while this variable is on the stack
        pin_ptr<Byte> pinnedBytes = &encodedBytes[0];
        auto map_constructor = PyObject_GetAttrString(halomap_class, "from_file");
        // cast pin_ptr to char*
        auto map = PyObject_CallObject(map_constructor,
            PyTuple_Pack(1, PyUnicode_FromString(reinterpret_cast<char*>(pinnedBytes))));
        this->maps->Add(gcnew HaloMapProxy(map));
    }

    String^ PythonInterpreter::ToString()
    {
        return String::Format("PythonInterpreter with {0} open maps.", this->maps->Count);
    }
}
