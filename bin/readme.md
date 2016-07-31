# Nimbus

Nimbus is a library for Python 3 which provides scriptable editing of Halo mapfiles, either on disk or in Halo's memory. Nimbus is compatible with Halo 1 PC on Windows.

## Usage

As an example of modding at runtime, here we swap the projectile of the player's starting weapon with a more powerful projectile.

### mem_example.py
```python
import nimbus
map = nimbus.HaloMap.from_memory()

# weapons
pistol = map.tag('weap', '\\pistol')
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

The entirety of my work on this project is released under the 2-clause BSD license. While contributors retain copyright on their own work, I ask that pull requests also be released under the 2-clause BSD license.

## Community

"If I have seen further it is by standing on the shoulders of giants." --Issac Newton

### People

- conure: Proved that runtime edits are possible
- Modzy: Provided source code to Open Halo Parser and Pearl 2
- Oxide: Provided a simple C++ map parsing example and source code to [Phasor](https://github.com/urbanyoung/Phasor)
- Ryx: Provided x86 assembly which prevents Halo's graphics from pausing when alt-tabbed
- dirk: Provided source code to [SDMHaloMapLoader](https://github.com/samdmarshall/SDMHaloMapLoader)
- Zero2: Explained reflexives, map magic, and map deprotection theory
- Btcc22: Assistance with C++ and reverse-engineering of the Halo executable

### Forums

- www.opencarnage.net
- www.macgamingmods.com/forums
- www.halomods.com
- www.xboxchaos.com/forum/
- www.modhalo.net
