# halolib.py

Halolib is a library for Python 3 which provides scriptable editing of Halo mapfiles, either on disk or in Halo's memory.

Halolib aims to be compatible with Halo 1 PC on Windows, and HaloMD on Mac OS X.

## Installation

Halolib requires an installation of Python 3. Either invoke the examples directly, or import from halolib.

If you do not already have an installation of Python 3, you can download my stripped-down portable Python 3.2 installation from [here](http://www.mediafire.com/download/55o5dzct6hyw8bd/halolib-portable-python-2013-09-06.7z) (15 MB zipped, 60 MB unzipped) and place the bin/ folder in the root of this repo.

If you are not executing halolib from a 32-bit installation of Python, you may need to recompile halolib/field.pyx. See Development below for compilation instructions.

## Usage

As an example of modding at runtime, here we swap the projectile of the player's starting weapon with a more powerful projectile.

### mem_example.py
```python
from halolib import *
load_plugins('.\plugins')
m = load_map_from_memory(fix_video_render=True)

# weaps
rifle = m.get_tag('weap', 'assault rifle')
banshee_gun = m.get_tag('weap', 'banshee')

# vehis
warthog = m.get_tag('vehi', 'warthog')

# projs
rocket = m.get_tag('proj', 'rocket')
plasma = m.get_tag('proj', 'plasma grenade')

# do swaps
rifle.triggers[0].projectile = rocket           # assault rifle shoots rockets
banshee_gun.triggers[0].projectile = warthog    # banshee primary trigger spawns warthogs
banshee_gun.triggers[1].projectile = plasma     # banshee secondary trigger shoots plasma grenades
```

![Effects of mem_example.py](http://i.imgur.com/tdnHwf0.png)

## Development

Halolib is developed on Windows with the 32-bit build of Python 3.2.

Building halolib/field.pyx requires Cython and a C compiler. I use a portable Python 3.2, Cython 0.19.1, and MinGW bundle that can be downloaded [here](http://www.mediafire.com/download/u1p4449zk4d2gy1/halolib-portable-devenv-2013-09-06.7z) (26 MB zipped, 82 MB unzipped). Place the bin/ folder in the root of this repository, and run halolib/build.bat to compile field.pyx.

#### Merging changes from byteaccess.py

Since the content of byteaccess/ seems useful even outside the context of Halo hacking, it has been split into a [separate git repo](https://github.com/ChadSki/byteaccess.py). Synchronizing changes between the two repos is managed with git subtree. To merge changes from byteaccess-origin, run the following commands:
```
git fetch byteaccess-origin
# http://stackoverflow.com/a/12048161/1628916
git merge -s ours --no-commit byteaccess-origin/master
```

### License

The entirety of my work on this project is released under the 2-clause BSD license. While contributors retain copyright on their own work, I ask that pull requests also be released under the 2-clause BSD license.

## Community

"If I have seen further it is by standing on the shoulders of giants." --Issac Newton

### People

- conure: Proved that runtime edits are possible
- Modzy: Provided source code to Open Halo Parser and Pearl 2
- Oxide: Provided a simple C++ map parsing example and source code to [Phasor](https://github.com/urbanyoung/Phasor)
- Ryx: Provided x86 assembly which prevents Halo's graphics from pausing when alt-tabbed
- dirk: Provided source code to [SDMHaloMapLoader](https://github.com/samdmarshall/SDMHaloMapLoader)
- nil: Reminded me that I don't need C++ to bridge Python and C#
- Zero2: Explained reflexives, map magic, and map deprotection theory
- Btcc22: Provided code to hook into the Halo console and add custom commands

If you helped me at one point in time and I forgot to list you, let me know so I can bestow proper credit!

### Forums

- www.opencarnage.net
- www.macgamingmods.com/forums
- www.halomods.com
- www.xboxchaos.com/forum/
- www.modhalo.net
