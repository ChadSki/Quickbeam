#pragma once
#include "Stdafx.h"
#include "PythonBinding.h"

using namespace System;

namespace PythonBinding {

    public enum class HaloMemory { PC, CE };

    ref class PythonInterpreter
    {
    public:
        static property PythonInterpreter^ Instance { PythonInterpreter^ get() { return %m_instance; } }
        PythonBinding::HaloMapProxy^ OpenMap(HaloMemory whichExe);
        PythonBinding::HaloMapProxy^ OpenMap(String^ filename);

    private:
        PythonInterpreter();
        PythonInterpreter(const PythonInterpreter%) { throw gcnew InvalidOperationException("PythonInterpreter cannot be copy-constructed"); }
        static PythonInterpreter m_instance;
    };

}
