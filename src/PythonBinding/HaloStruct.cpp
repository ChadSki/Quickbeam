#include "stdafx.h"
#include "HaloStruct.h"

String^ ManagedString(PyObject* pyo)
{
    auto utf8bytes = PyUnicode_AsUTF8AndSize(PyObject_Str(pyo), nullptr);
    return gcnew String(utf8bytes);
}

namespace PythonBinding
{
    String^ Field::Name::get() { return name; }

    Field::Field(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    String^ BytesField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto fieldValue = PyObject_CallObject(set_fn, nullptr);
        return ManagedString(fieldValue);
    }

    void BytesField::Value::set(String^ newvalue)
    {
        //
    }

    double FloatField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto fieldValue = PyObject_CallObject(set_fn, nullptr);
        return PyFloat_AsDouble(fieldValue);
    }

    void FloatField::Value::set(double newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyFloat_FromDouble(newvalue)));
    }

    long IntField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto fieldValue = PyObject_CallObject(set_fn, nullptr);
        return PyLong_AsLong(fieldValue);
    }

    void IntField::Value::set(long newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyLong_FromLong(newvalue)));
    }

    String^ StringField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto fieldValue = PyObject_CallObject(set_fn, nullptr);
        return ManagedString(fieldValue);
    }

    void StringField::Value::set(String^ newvalue)
    {
        std::cout << "Get set_fn" << std::endl;
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        std::cout << "Get rawValue" << std::endl;
        auto rawValue = Marshal::StringToHGlobalAnsi(newvalue);
        std::cout << "Creating args" << std::endl;
        auto args = PyTuple_Pack(1, (char*)(void*)rawValue);
        std::cout << "Calling" << std::endl;
        PyObject_CallObject(set_fn, args);
        std::cout << "Freeing" << std::endl;
        Marshal::FreeHGlobal(rawValue);
    }

    String^ UnknownField::Value::get()
    {
        // Get our string representation from parent
        auto parentStruct = PyObject_GetAttrString(this->field, "parent");
        auto rawName = Marshal::StringToHGlobalAnsi(this->name);
        auto fieldValue = PyObject_GetAttrString(parentStruct, (char*)(void*)rawName);
        Marshal::FreeHGlobal(rawName);
        return ManagedString(fieldValue);
    }

    HaloStructViewModel::HaloStructViewModel(PyObject* halostruct)
    {
        this->halostruct = new ObservablePyObject(halostruct);
        this->fields = gcnew ObservableCollection<Field^>();

        // For debug, but it's very noisy.
        //PyObject_Print(halostruct, stdout, Py_PRINT_RAW);
        //std::cout << std::endl;

        auto fieldsDict = PyObject_GetAttrString(halostruct, "fields");
        // type: Dict[str, Union[BasicField, HaloField]]

        auto fieldsItems = PyMapping_Items(fieldsDict);
        // type: Sequence[Tuple[str, Union[BasicField, HaloField]]]

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

            // Field
            auto fieldObj = PySequence_GetItem(item, 1);
            auto typeName = PyObject_GetAttrString(
                (PyObject*)fieldObj->ob_type, "__name__");
            auto typeNameChar = PyUnicode_AsUTF8AndSize(typeName, nullptr);
            auto typeNameStr = gcnew String(typeNameChar);

            if (typeNameStr->Contains("RawData"))
            {
                this->Fields->Add(gcnew BytesField(fieldNameStr, fieldObj));
            }
            else if (typeNameStr->Contains("Float"))
            {
                this->Fields->Add(gcnew FloatField(fieldNameStr, fieldObj));
            }
            else if (typeNameStr->Contains("Int"))
            {
                this->Fields->Add(gcnew IntField(fieldNameStr, fieldObj));
            }
            else if (typeNameStr->Contains("Ascii"))
            {
                this->Fields->Add(gcnew StringField(fieldNameStr, fieldObj));
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
        if (result == nullptr) {
            throw gcnew NullReferenceException(
                String::Format("Could not find attribute `{}`", attrName));
        }
        Marshal::FreeHGlobal(attrNameC);
        auto charArray = PyBytes_AsString(PyUnicode_AsASCIIString(result));
        return gcnew String(charArray);
    }

    String^ HaloStructViewModel::ToString()
    {
        return String::Format("HaloStruct with {0} fields.", this->Fields->Count);
    }
}