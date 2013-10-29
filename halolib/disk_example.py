import halolib
m = halolib.load_map("beavercreek.map")

print(repr(m))
for tag in m.get_tags('weap'):
    pass #print(tag.layout)

t = m.get_tag('bipd', '')
print(repr(t))

def print_data(x):
    print(x.struct_data)
    for key in x.reflexives:
        for each in x.reflexives[key]:
            print_data(each)

m.close()
