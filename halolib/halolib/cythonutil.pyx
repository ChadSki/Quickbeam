# Copyright (c) 2013, Chad Zawistowski
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#     * Redistributions of source code must retain the above copyright
#       notice, this list of conditions and the following disclaimer.
#     * Redistributions in binary form must reproduce the above copyright
#       notice, this list of conditions and the following disclaimer in the
#       documentation and/or other materials provided with the distribution.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
# ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

"""cythonutil.pyx

This file contains Cython declarations at the Python level, and is combined with
cythonutil.pxd at compile-time.
"""

def py_strlen(s):
    """Returns the length of the specified string up to the first null terminator.
    Contrast with the built-in Python len(), which return the length of a string
    including all null values."""
    return strlen(<char*>s)

# Get primitive values from pointer location
# ----------------------------------------------------------------
def read_float32(int address): return (<float*>address)[0]
def read_float64(int address): return (<double*>address)[0]
def read_int8(int address):    return (<int8_t*>address)[0]
def read_int16(int address):   return (<int16_t*>address)[0]
def read_int32(int address):   return (<int32_t*>address)[0]
def read_int64(int address):   return (<int64_t*>address)[0]
def read_uint8(int address):   return (<uint8_t*>address)[0]
def read_uint16(int address):  return (<uint16_t*>address)[0]
def read_uint32(int address):  return (<uint32_t*>address)[0]
def read_uint64(int address):  return (<uint64_t*>address)[0]

# Assign primitive values to pointer location
# ----------------------------------------------------------------
def write_float32(int address, float value):    (<float*>address)[0] = value
def write_float64(int address, float value):    (<double*>address)[0] = value
def write_int8(int address,    int8_t value):   (<int8_t*>address)[0] = value
def write_int16(int address,   int16_t value):  (<int16_t*>address)[0] = value
def write_int32(int address,   int32_t value):  (<int32_t*>address)[0] = value
def write_int64(int address,   int64_t value):  (<int64_t*>address)[0] = value
def write_uint8(int address,   uint8_t value):  (<uint8_t*>address)[0] = value
def write_uint16(int address,  uint16_t value): (<uint16_t*>address)[0] = value
def write_uint32(int address,  uint32_t value): (<uint32_t*>address)[0] = value
def write_uint64(int address,  uint64_t value): (<uint64_t*>address)[0] = value
