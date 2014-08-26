## Startup script for IronPython engine

quickbeam_dir = r"C:\Users\chzawist\Src\Quickbeam"
stdlib_dir = quickbeam_dir + r"\vendor\PythonStdLib"
dlls_dir = quickbeam_dir + r"\src\bin\Debug"
userlib_dir = quickbeam_dir + r"\src"

import sys
sys.path.append(stdlib_dir)
sys.path.append(dlls_dir)
sys.path.append(userlib_dir)

import os
# Set properties for proper behaviour with PyDoc
os.environ['TERM'] = 'dumb'
