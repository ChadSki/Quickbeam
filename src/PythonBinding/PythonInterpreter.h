#pragma once
ref class PythonInterpreter
{
private:
    PythonInterpreter();
    PythonInterpreter(const PythonInterpreter%) { throw gcnew System::InvalidOperationException("PythonInterpreter cannot be copy-constructed"); }
    static PythonInterpreter m_instance;

public:
    static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %m_instance; } }
};

