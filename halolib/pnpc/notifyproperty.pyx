from libc.stdint cimport int8_t, int16_t, int32_t, int64_t
from libc.stdint cimport uint8_t, uint16_t, uint32_t, uint64_t

cdef extern from "funcptr.h" nogil:
    int default_callback(void* opaque, char* name)
    int exec_callback(void* callback_fn, void* opaque, char* name)

def notify_property(name):
    """Works just like a regular property, but additionally triggers the host
    class's on_property_changed event when the value is changed.

    Saves the name of the property for each individual property-derived class; a new
    NotifyProperty class is generated for each name.
    """
    class NotifyProperty(property):
        """See pnpc.notify_property"""
        def __init__(self, fget=None, fset=None, fdel=None, doc=None, *args, **kwargs):
            def newgetter(slf):
                try:
                    return fget(slf)
                except AttributeError:
                    return None

            super(NotifyProperty, self).__init__(
                fget=newgetter, fset=fset, fdel=fdel, doc=doc, *args, **kwargs)

        def setter(self, setter):
            def newsetter(slf, newvalue):
                oldvalue = self.fget(slf)
                if oldvalue != newvalue:
                    setter(slf, newvalue)
                    slf.on_property_changed(name)

            return NotifyProperty(
                fget=self.fget, fset=newsetter, fdel=self.fdel, doc=self.__doc__)

    return NotifyProperty

def simple_notify_property(property_name):
    """Rather than decorate a custom getter and setter, this generates a
    notify_property which wraps a simple '_'-prefixed private member.
    """
    @notify_property(property_name)
    def foo(self): return getattr(self, '_' + property_name)
    @foo.setter
    def foo(self, value): setattr(self, '_' + property_name, value)
    return foo

cdef class Pnpc:
    """See pnpc.PyNotifyPropertyChanged
    """
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

num_pnpc_classes = 0
def PyNotifyPropertyChanged(*args):
    """Defines class which implements the INotifyPropertyChanged interface.
    Autogenerates simple notify-properties named by the provided arguments.
    """
    global num_pnpc_classes
    CustomPnpc = type('CustomPnpc%d' % num_pnpc_classes, (Pnpc,), {
        name: simple_notify_property(name) for name in args
        })
    num_pnpc_classes +=1
    return CustomPnpc
