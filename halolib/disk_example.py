import halolib
import time
m = halolib.load_map("./beavercreek.map")
#m = halolib.load_map("./bloodgulch_soft.map")

for each in m.get_tags('bipd|weap'):
    print(repr(each))

t = m.get_tag('bipd', '')
print(repr(t))

t.turn_speed *= 1.05

print(m.get_tag('bipd'))
print()

m.asdf = 123

m.close()
