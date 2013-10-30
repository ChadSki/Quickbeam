from pnpc.notifyproperty import PyNotifyPropertyChanged

__version__ = 'TODO: Figure out Python 3 version conventions'


def notify_property(name):
    class NotifyProperty(property):
        """Works just like a regular property, but additionally triggers the host
        class's on_property_changed event when the value is changed.

        Saves the name of the property in a closure around the class; a new
        NotifyProperty class is generated for each property.
        """
        def __init__(self, fget=None, fset=None, fdel=None, doc=None, *args, **kwargs):
            def newgetter(slf):
                try:
                    return fget(slf)
                except AttributeError:
                    return None

            super(NotifyProperty, self).__init__(fget=newgetter, fset=fset, fdel=fdel, doc=doc, *args, **kwargs)

        def setter(self, setter):
            def newsetter(slf, newvalue):
                oldvalue = self.fget(slf)
                if oldvalue != newvalue:
                    setter(slf, newvalue)
                    slf.on_property_changed(name)

            return NotifyProperty(
                fget=self.fget,
                fset=newsetter,
                fdel=self.fdel,
                doc=self.__doc__)

    return NotifyProperty