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

from plugins import plugin_classes

import clr
clr.AddReference("Quickbeam.Low.dll")
from observablefield import AsciizField

class HaloTag(object):
    """Encapsulates an index entry, tag name, and metadata."""
    def __init__(self, index_entry, name_access, meta_access, halomap):
        # these attributes are all protected from erroneous assignment
        object.__setattr__(self, 'index_entry', index_entry)
        object.__setattr__(self, 'name_field', None) #AsciizField(halomap.CreateByteArray(name_access, 0, name_access.Size))
        object.__setattr__(self, 'meta_access', meta_access)
        object.__setattr__(self, 'halomap', halomap)

    @property
    def name(self):
        return name_field.Value
    @name.setter
    def name(self, value):
        name_field.Value = value

    @property
    def meta(self):
        # every time the meta is accessed, reinterpret it as the current first_class
        try:
            return plugin_classes[self.first_class](self.meta_access, self.halomap)
        except KeyError:
            return plugin_classes['unknown'](self.meta_access, self.halomap)
    @meta.setter
    def meta(self, value):
        raise Exception('Replacing entire meta at once not yet implemented.')

    def __str__(self):
        """Returns a 1-line string representation of this tag."""
        return '[%s]%s(%d)' % (self.first_class, self.name, self.ident)

    def __repr__(self):
        """Returns a full string representation of this tag and its metadata."""
        return str(self) + str(self.meta)

    def __getattr__(self, name):
        """HaloTag (using magic) sort of merges the attributes of self.index_entry and self.meta alongside its own.

        Getting an attribute resolves in the following order:
            1. First self's attributes are checked
            2. If nothing was found, Python will run __getattr__ and check self.index_entry's attributes
            3. If nothing was found in self.index_entry, check self.meta
            4. If checking self.meta fails just let the AttributeError propagate upwards
        """
        try:
            return getattr(self.index_entry, name)
        except AttributeError:
            return getattr(self.meta, name)

    def __setattr__(self, name, value):
        """HaloTag (using magic) sort of merges the attributes of self.index_entry and self.meta alongside its own.

        Setting an attribute resolves in the following order:
            1. [list of attributes] are exempt from being replaced
            2. Other attributes of self can be assigned to
            3. If the attribute is not found in self, check self.index_entry
            4. If the attribute is not found in self.index_entry, check self.meta
            5. If checking self.meta fails, raise AttributeError
        """
        if name in ['meta', 'index_entry', 'name_field', 'meta_access', 'halomap']:
            raise Exception('%s is protected from erroneous assignment.' % name)

        elif name in self.__dict__:
            object.__setattr__(self, name, value)

        elif hasattr(self.index_entry, name):
            setattr(self.index_entry, name, value)

        elif hasattr(self.meta, name):
            setattr(self.meta, name, value)

        else:
            raise AttributeError('Not allowed to create new attributes on HaloTag.')
