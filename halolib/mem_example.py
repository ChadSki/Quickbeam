import halolib
m = halolib.load_map()

#print(repr(m))
print(repr(m.get_tag('bipd')))

# weaps
rifle = m.get_tag('weap', 'assault rifle')
pistol = m.get_tag('weap', '\\pistol')
banshee_gun = m.get_tag('weap', 'banshee')

# vehis
banshee = m.get_tag('vehi', 'banshee')
warthog = m.get_tag('vehi', 'warthog')

# projs
charged = m.get_tag('proj', 'charged')
rocket = m.get_tag('proj', 'rocket')
plasma = m.get_tag('proj', 'plasma grenade')
plasma.initial_velocity = 0.2
plasma.final_velocity = 0.2

# do swaps
rifle.triggers[0].projectile = charged

print(rifle)
