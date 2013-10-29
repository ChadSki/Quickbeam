class notify_property(property):
    def __init__(self, getter):
        def newgetter(slf):
            try:
                return getter(slf)
            except AttributeError:
                return None
        super(notify_property, self).__init__(newgetter)

    def setter(self, setter):
        def newsetter(slf, newvalue):
            oldvalue = self.fget(slf)
            if oldvalue != newvalue:
                setter(slf, newvalue)
                slf.OnPropertyChanged(setter.__name__)
        return property(
            fget=self.fget,
            fset=newsetter,
            fdel=self.fdel,
            doc=self.__doc__)
