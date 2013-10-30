@echo off

REM Adjust paths
pushd %~dp0
set HOME=..\
REM Python (with Cython installed) and MinGW should be on your %PATH%

del *.pyd
python .\setup.py build_ext --inplace
del *.c

REM Restore cwd
popd
