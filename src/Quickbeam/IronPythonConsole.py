## Startup script for IronPython engine

stdlib_dir = r"C:\Users\Chad\Source Code\Quickbeam\vendor\PythonStdLib"
dlls_dir = r"C:\Users\Chad\Source Code\Quickbeam\src\bin\Debug"
userlib_dir = r"C:\Users\Chad\Source Code\Quickbeam\src"

import sys
sys.path.append(stdlib_dir)
sys.path.append(dlls_dir)
sys.path.append(userlib_dir)

import os
# Set properties for proper behaviour with PyDoc
os.environ['TERM'] = 'dumb'
