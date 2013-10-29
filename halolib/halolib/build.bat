@echo off
pushd %~dp0
SET VS90COMNTOOLS=%VS110COMNTOOLS%
SET VS100COMNTOOLS=%VS110COMNTOOLS%
SET HOME=..\
SET PATH=..\;..\..\vendor\cpython\PCbuild;%PATH%
SET PYTHONPATH=..\;..\..\vendor\cpython\PCbuild;%PYTHONPATH%
DEL *.pyd
python .\setup.py build_ext --inplace
DEL *.c
popd
