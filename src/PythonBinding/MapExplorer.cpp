// This is the main DLL file.

#include "stdafx.h"
#include "MapExplorer.h"

namespace PythonBinding {

    HaloTagNode::HaloTagNode(PyObject* halotag)
    {
        this->halotag = halotag;
        this->header = gcnew HaloStructViewModel(PyObject_GetAttrString(this->halotag, "header"));
        this->data = gcnew HaloStructViewModel(PyObject_GetAttrString(this->halotag, "data"));
        this->noChildren = gcnew ChildNodes();
    }

    String^ HaloTagNode::FirstClass::get()
    {
        return (String^)(header->Get("first_class"));
    }

    String^ HaloTagNode::Name::get()
    {
        return (String^)(header->Get("name"));
    }

    String^ HaloTagNode::Suffix::get()
    {
        return "tag";
    }

    ChildNodes^ HaloTagNode::Children::get()
    {
        return noChildren;
    }

    String^ HaloTagNode::ToString()
    {
        return gcnew String("HaloTag.");
    }

    HaloTagClassNode::HaloTagClassNode(String^ className, ChildNodes^ tags)
    {
        this->className = className;
        this->tags = tags;
    }

    String^ HaloTagClassNode::Name::get()
    {
        return className;
    }

    ChildNodes^ HaloTagClassNode::Children::get()
    {
        return tags;
    }

    // For sorting tag classes
    String^ Identity(String^ x) { return x; }

    HaloMapNode::HaloMapNode(PyObject* map)
    {
        this->halomap = map;
        this->tagClasses = gcnew ChildNodes();

        auto tags_fn = PyObject_GetAttrString(this->halomap, "tags");
        auto allTags = PyObject_CallObject(tags_fn, nullptr);
        auto tagsIter = PyObject_GetIter(allTags);
        if (tagsIter == nullptr) {
            throw gcnew NullReferenceException(
                "Could not iterate over tags.");
        }

        // Read all the tags into our collection
        PyObject* item;
        auto tagsByClass = gcnew Dictionary<String^, ChildNodes^>();
        while (item = PyIter_Next(tagsIter)) {
            auto tag = gcnew HaloTagNode(item);
            auto tagClassStr = tag->FirstClass;
            if (!tagsByClass->ContainsKey(tagClassStr))
            {
                tagsByClass->Add(tagClassStr, gcnew ChildNodes());
            }
            (tagsByClass[tagClassStr])->Add(tag);
        }
        auto sortedClasses = Enumerable::OrderBy(tagsByClass->Keys,
            gcnew Func<String^, String^>(Identity));

        for each (auto tagClass in sortedClasses)
        {
            tagClasses->Add(gcnew HaloTagClassNode(tagClass, tagsByClass[tagClass]));
        }
    }

    HaloTagNode^ HaloMapNode::getArbitraryTag()
    {
        auto tag_fn = PyObject_GetAttrString(this->halomap, "tag");
        auto args = PyTuple_Pack(2,
            PyUnicode_FromString("ant!"),
            PyUnicode_FromString(""));
        auto ghost = PyObject_CallObject(tag_fn, args);
        return gcnew HaloTagNode(ghost);
    }

    ChildNodes^ HaloMapNode::Children::get()
    {
        return tagClasses;
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
        if (sys_mod_dict == nullptr) {
            throw gcnew NullReferenceException(
                "Could not open Python module dictionary.");
        }
        auto main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
        if (main_mod == nullptr) {
            throw gcnew NullReferenceException(
                "Could not open __main__ module.");
        }
        auto halolib = PyObject_GetAttrString(main_mod, "halolib");
        if (halolib == nullptr) {
            throw gcnew NullReferenceException(
                "Could not find library `halolib`.");
        }
        auto halolib_dict = PyModule_GetDict(halolib);
        if (halolib == nullptr) {
            throw gcnew NullReferenceException(
                "Could not get dictionary from halolib.");
        }
        auto halomap_class = PyDict_GetItem(halolib_dict, PyUnicode_FromString("HaloMap"));
        if (halomap_class == nullptr) {
            throw gcnew NullReferenceException(
                "Could not get HaloMap class from halolib.");
        }
        this->halomap_class = halomap_class;
        this->maps = gcnew ChildNodes();
    }

    PyObj^ PythonInterpreter::Initialize()
    {
        Py_Initialize();
        PyRun_SimpleString(
            "import sys\n"
            //"sys.stdout = open('CONOUT$', 'wt')\n"  // Fix console output
            "import halolib\n");

        auto sys_mod_dict = PyImport_GetModuleDict();
        if (sys_mod_dict == nullptr) {
            throw gcnew NullReferenceException(
                "Could not open Python module dictionary.");
        }
        auto main_mod = PyMapping_GetItemString(sys_mod_dict, "__main__");
        if (main_mod == nullptr) {
            throw gcnew NullReferenceException(
                "Could not open __main__ module.");
        }
        return gcnew PyObj(main_mod);
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

    ChildNodes^ PythonInterpreter::Children::get()
    {
        return maps;
    }

    String^ PythonInterpreter::ToString()
    {
        return String::Format("PythonInterpreter with {0} open maps.", this->maps->Count);
    }
}
