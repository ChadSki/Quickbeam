# Start up script for IronPythonConsole

stdlib_dir = r"C:\Dropbox\Workbench\CodeProjects\HaloFiles\Source Code\Quickbeam\vendor\PythonStdLib"
dlls_dir = r"C:\Dropbox\Workbench\CodeProjects\HaloFiles\Source Code\Quickbeam\src\bin\Debug"
halolib_dir = r"C:\Dropbox\Workbench\CodeProjects\HaloFiles\Source Code\Quickbeam\src\halolib"

import sys
sys.path.append(stdlib_dir)
sys.path.append(dlls_dir)
sys.path.append(halolib_dir)

import os
# Set properties for proper behaviour with PyDoc
os.environ['TERM'] = 'dumb'
