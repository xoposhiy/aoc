from aoc import *

class KdTree:
    def __init__(self, bbox, axis=0, full_value=None):
        self.bbox = bbox
        self.axis = axis
        self.full_value = full_value
        self.left = None
        self.right = None
        self.divider = None

    def set(self, cuboid):
        if self.bbox == cuboid:
            self.full_value = True
            self.left = None
            self.right = None
            return
        if self.full_value:
            return
        if self.divider is None:
            if cuboid[self.axis*2] != self.bbox[self.axis*2]:
                self.divider = cuboid[self.axis*2]
            else:
                self.divider = cuboid[self.axis*2+1]+1
        self.create_children()
        left, right = self.divide(cuboid)
        if left is not None:
            self.left.set(left)
        if right is not None:
            self.right.set(right)
        self.full_value = None

    def unset(self, cuboid):
        if self.bbox == cuboid:
            self.full_value = False
            self.left = None
            self.right = None
            return
        if self.full_value:
            if cuboid[self.axis*2] != self.bbox[self.axis*2]:
                self.divider = cuboid[self.axis*2]
            else:
                self.divider = cuboid[self.axis*2+1]+1
            self.create_children()
            self.left.full_value = True
            self.right.full_value = True
            self.full_value = None

        if self.left is None:
            return

        left, right = self.divide(cuboid)
        if left is not None:
            self.left.unset(left)
        if right is not None:
            self.right.unset(right)

    def count_pixels(self, container):
        if self.full_value is not None:
            return cube_size(container) if self.full_value else 0
        if self.left is None:
            return 0
        left, right = self.divide(container)
        res = 0
        if self.left is not None and left is not None:
            res += self.left.count_pixels(left)
        if self.right is not None and right is not None:
            res += self.right.count_pixels(right)
        return res


    def divide(self, container):
        x0 = container[self.axis*2]
        x1 = container[self.axis*2+1]
        if x0 >= self.divider:
            return None, container
        if x1 < self.divider:
            return container, None
        left = container.copy()
        right = container.copy()
        left[self.axis*2+1] = self.divider-1
        right[self.axis*2] = self.divider
        return left, right

    def create_children(self):
        if self.left is None:
            left, right = self.divide(self.bbox)
            ax = (self.axis + 1) % 3
            self.left = KdTree(left, ax)
            self.right = KdTree(right, ax)


def cube_size(c):
    sx = max(0, c[1] - c[0] + 1)
    sy = max(0, c[3] - c[2] + 1)
    sz = max(0, c[5] - c[4] + 1)
    return sx * sy * sz


universe = [-200000, 200000, -200000, 200000, -200000, 200000]
cubes = KdTree(universe)

for turn, *cube in read(sep=" |x=|\\.\\.|,y=|,z="):
    if turn == 'on':
        cubes.set(cube)
    else:
        cubes.unset(cube)

print("Part One", cubes.count_pixels([-50, 50, -50, 50, -50, 50]))
print("Part Two", cubes.count_pixels(universe))
