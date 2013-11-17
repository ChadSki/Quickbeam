import halolib
import time
m = halolib.load_map("./beavercreek.map")

#print(repr(m))

t = m.get_tag('bipd', '')
print(repr(t))

t.turn_speed *= 1.05

def print_data(x):
    print(x.struct_data)
    for key in x.reflexives:
        for each in x.reflexives[key]:
            print_data(each)

print(m.get_tag('bipd'))

m.asdf = 123

m.close()
