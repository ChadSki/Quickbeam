@echo off

REM Adjust paths
pushd %~dp0
SET HOME=..\
SET PATH=..\;..\..\vendor\cpython\PCbuild;%PATH%
SET PYTHONPATH=..\;..\..\vendor\cpython\PCbuild;%PYTHONPATH%

REM Force Visual Studio 2012 compiler
SET VS90COMNTOOLS=%VS110COMNTOOLS%
SET VS100COMNTOOLS=%VS110COMNTOOLS%

DEL *.pyd
python .\setup.py build_ext --inplace
DEL *.c

REM Restore cwd
popd
