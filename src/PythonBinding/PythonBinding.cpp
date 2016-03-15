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

    HaloStructViewModel::HaloStructViewModel(PyObject* halostruct)
    {
        this->halostruct = new ObservablePyObject(halostruct);
        this->fields = gcnew List<Field^>();

        PyObject_Print(halostruct, stdout, Py_PRINT_RAW);
        std::cout << std::endl;

        // type: Dict[str, Union[BasicField, HaloField]]
        auto fieldsDict = PyObject_GetAttrString(halostruct, "fields");
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

            // read value
            //auto fieldValue = PyObject_GetAttrString(this->halostruct->pyobj, fieldNameChar);
            //auto fieldValueChar = PyUnicode_AsUTF8AndSize(PyObject_Str(fieldValue), nullptr);
            //auto fieldValueStr = gcnew String(fieldValueChar);

            // Field
            auto fieldObj = PySequence_GetItem(item, 1);
            auto typeName = PyObject_GetAttrString(
                (PyObject*)fieldObj->ob_type, "__name__");
            auto typeNameChar = PyUnicode_AsUTF8AndSize(typeName, nullptr);
            auto typeNameStr = gcnew String(typeNameChar);

            if (typeNameStr->Contains("Float"))
            {
                this->Fields->Add(gcnew FloatField(fieldNameStr, fieldObj));
            }
            else
            {
                this->Fields->Add(gcnew UnknownField(fieldNameStr, fieldObj));
            }
        }
    }

    Object^ HaloStructViewModel::Get(String^ attrName)
    {
        auto attrNameC = Marshal::StringToHGlobalAnsi(attrName);
        auto result = PyObject_GetAttrString(halostruct->pyobj, (const char*)attrNameC.ToPointer());
        Marshal::FreeHGlobal(attrNameC);
        auto charArray = PyBytes_AsString(PyUnicode_AsASCIIString(result));
        return gcnew String(charArray);
    }

    String^ HaloStructViewModel::ToString()
    {
        return String::Format("HaloStruct with {0} fields.", this->Fields->Count);
    }

    HaloTagNode::HaloTagNode(PyObject* halotag)
    {
        this->halotag = halotag;
        this->header = gcnew HaloStructViewModel(PyObject_GetAttrString(this->halotag, "header"));
        this->data = gcnew HaloStructViewModel(PyObject_GetAttrString(this->halotag, "data"));
        this->noChildren = gcnew List<ExplorerNode^>();
    }

    String^ HaloTagNode::ToString()
    {
        return gcnew String("HaloTag.");
    }

    HaloMapNode::HaloMapNode(PyObject* map)
    {
        this->halomap = map;
        this->tagClasses = gcnew List<ExplorerNode^>();

        auto tags_fn = PyObject_GetAttrString(this->halomap, "tags");
        auto allTags = PyObject_CallObject(tags_fn, nullptr);
        auto tagsIter = PyObject_GetIter(allTags);
        if (tagsIter == nullptr) {
            throw gcnew NullReferenceException(
                "Could not iterate over tags.");
        }

        // Read all the tags into our collection
        PyObject* item;
        auto tagsByClass = gcnew Dictionary<String^, List<ExplorerNode^>^>();
        while (item = PyIter_Next(tagsIter)) {
            auto tag = gcnew HaloTagNode(item);
            auto tagClassStr = tag->FirstClass;
            if (!tagsByClass->ContainsKey(tagClassStr))
            {
                tagsByClass->Add(tagClassStr, gcnew List<ExplorerNode^>());
            }
            (tagsByClass[tagClassStr])->Add(tag);
        }
        auto sortedClasses = gcnew List<String^>(tagsByClass->Keys);
        sortedClasses->Sort();
        for each (auto tagClass in sortedClasses)
        {
            tagClasses->Add(gcnew HaloTagClassNode(tagClass, tagsByClass[tagClass]));
        }
    }

    HaloTagNode^ HaloMapNode::getGhost()
    {
        auto tag_fn = PyObject_GetAttrString(this->halomap, "tag");
        auto args = PyTuple_Pack(2,
            PyUnicode_FromString("vehi"),
            PyUnicode_FromString("ghost"));
        auto ghost = PyObject_CallObject(tag_fn, args);
        return gcnew HaloTagNode(ghost);
    }

    String^ HaloMapNode::ToString()
    {
        return String::Format("HaloMap with {0} tag classes.", this->tagClasses->Count);
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
        this->maps = gcnew List<ExplorerNode^>();
        
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
        if (map == nullptr) {
            throw gcnew NullReferenceException(
                "Could not open map.");
        }
        this->maps->Add(gcnew HaloMapNode(map));
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
        this->maps->Add(gcnew HaloMapNode(map));
    }

    String^ PythonInterpreter::ToString()
    {
        return String::Format("PythonInterpreter with {0} open maps.", this->maps->Count);
    }
}
