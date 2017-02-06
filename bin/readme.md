# Nimbus

Nimbus is a library for Python 3 which provides scriptable editing of Halo mapfiles, either on disk or in Halo's memory. Nimbus is compatible with Halo 1 PC on Windows.

## Usage

As an example of modding at runtime, here we swap the projectile of the player's starting weapon with a more powerful projectile.

### mem_example.py
```python
import nimbus
map = nimbus.HaloMap.from_memory()

# weapons
pistol = map.tag('weap', '\\pistol') # include a slash so we don't get the plasma pistol
rifle = map.tag('weap', 'assault rifle')
banshee_gun = map.tag('weap', 'banshee')

# vehicles
warthog = map.tag('vehi', 'warthog')

# projectiles
rocket = map.tag('proj', 'rocket')
plasma = map.tag('proj', 'plasma grenade')

# make edits
pistol.triggers[0].projectile = rocket        # pistol now shoots rockets
rifle.triggers[0].projectile = rocket         # assault rifle now shoots rockets
banshee_gun.triggers[0].projectile = warthog  # banshee primary trigger now spawns warthogs
banshee_gun.triggers[1].projectile = plasma   # banshee secondary trigger now shoots plasma grenades
```

![Effects of mem_example.py](http://i.imgur.com/tdnHwf0.png)

### License

This program is free software; you can redistribute it and/or modify it under
the terms of the GNU General Public License as published by the Free Software
Foundation; either version 2 of the License, or (at your option) any later
version.

### Thanks

- conure: Proved that runtime edits are possible
- Modzy: Provided source code to Open Halo Parser and Pearl 2
- Oxide: Provided a simple C++ map parsing example and source code to [Phasor](https://github.com/urbanyoung/Phasor)
- Ryx: Provided x86 assembly which prevents Halo's graphics from pausing when alt-tabbed
- dirk: Provided source code to [SDMHaloMapLoader](https://github.com/samdmarshall/SDMHaloMapLoader)
- nil: Always a big help on IRC
- Zero2: Explained reflexives, map magic, and map deprotection theory
- Btcc22: Provided code to hook into the Halo console and add custom commands
- Xerax: Author of [Assembly](https://github.com/XboxChaos/Assembly)'s excellent WPF GUI.

### Forums

- www.opencarnage.net
- www.macgamingmods.com/forums
- www.halomods.com
- www.xboxchaos.com/forum/
- www.modhalo.net
