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

from System.ComponentModel import INotifyPropertyChanged

class PyNotifyPropertyChanged(INotifyPropertyChanged):
    PropertyChanged = None

    def __init__(self):
        self.PropertyChanged, self._propertyChangedCaller = pyevent.make_event()

    def add_PropertyChanged(self, value):
        self.PropertyChanged += value

    def remove_PropertyChanged(self, value):
        self.PropertyChanged -= value

    def OnPropertyChanged(self, propertyName):
        if self.PropertyChanged is not None:
            self._propertyChangedCaller(self, PropertyChangedEventArgs(propertyName))


def notify_property(name):
    """Works just like a regular property, but additionally triggers the host
    class's OnPropertyChanged event when the value is changed.

    Saves the name of the property for each individual property-derived class; a new
    NotifyProperty class is generated for each name.
    """
    class NotifyProperty(property):
        def __init__(self, fget=None, fset=None, fdel=None, doc=None):
            def newgetter(slf):
                try:
                    return fget(slf)
                except AttributeError:
                    return None
            super(NotifyProperty, self).__init__(
                fget=newgetter, fset=fset, fdel=fdel, doc=doc)

        def setter(self, fset):
            def newsetter(slf, newvalue):
                oldvalue = self.fget(slf)
                if oldvalue != newvalue:
                    fset(slf, newvalue)
                    slf.OnPropertyChanged(fset.__name__)
            return NotifyProperty(
                fget=self.fget, fset=newsetter, fdel=self.fdel, doc=self.__doc__)

    return NotifyProperty
