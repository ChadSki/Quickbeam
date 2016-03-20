#include "stdafx.h"
#include "HaloStruct.h"

namespace PythonBinding
{
    String^ Field::Name::get() { return name; }

    FloatField::FloatField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    double FloatField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto value = PyObject_CallObject(set_fn, nullptr);
        return PyFloat_AsDouble(value);
    }

    void FloatField::Value::set(double newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyFloat_FromDouble(newvalue)));
    }

    IntField::IntField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    long IntField::Value::get()
    {
        auto set_fn = PyObject_GetAttrString(this->field, "getf");
        auto value = PyObject_CallObject(set_fn, nullptr);
        return PyLong_AsLong(value);
    }

    void IntField::Value::set(long newvalue)
    {
        auto set_fn = PyObject_GetAttrString(this->field, "setf");
        PyObject_CallObject(set_fn, PyTuple_Pack(1,
            PyLong_FromLong(newvalue)));
    }

    UnknownField::UnknownField(String^ name, PyObject* field)
    {
        this->name = name;
        this->field = field;
    }

    HaloStructViewModel::HaloStructViewModel(PyObject* halostruct)
    {
        this->halostruct = new ObservablePyObject(halostruct);
        this->fields = gcnew ObservableCollection<Field^>();

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
}