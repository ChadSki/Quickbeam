from libc.stdint cimport int8_t, int16_t, int32_t, int64_t
from libc.stdint cimport uint8_t, uint16_t, uint32_t, uint64_t

cdef extern from "funcptr.h" nogil:
    int default_callback(void* opaque, char* name)
    int exec_callback(void* callback_fn, void* opaque, char* name)

cdef class PyNotifyPropertyChanged:
    cdef:
        void* callback_fn
        void* opaque_arg

    def __init__(self, *args, **kwargs):
        self.register_callback(<uint32_t><void*>default_callback, 0)

    def register_callback(self, uint32_t callback_ptr, uint32_t opaque_ptr):
        self.callback_fn = <void*>callback_ptr
        self.opaque_arg = <void*>opaque_ptr

    def on_property_changed(self, name):
        print("%s was changed to %s" % (name, str(getattr(self, name))))
        b = name.encode('ascii')
        exec_callback(self.callback_fn, self.opaque_arg, <char*>b)
