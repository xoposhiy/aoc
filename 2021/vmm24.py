def vm(z, w, xx, yy, zz):
    x = 0 if z % 26 + xx == w else 1
    return (z // zz) * (25 * x + 1) + (w + yy) * x

def vm_z1(z, w, xx, yy):
    return z * 26 + w + yy
# target = z*26 + w + yy
# z = (target - w - yy)/26 if (target - w - yy)%26 == 0

#-11 5
def vm_z26(z, w, xx, yy):
    x = 0 if z % 26 + xx == w else 1
    return (z // 26) * (25 * x + 1) + (w + yy) * x
    # (z // 26) * (25 * x + 1) + (w + yy) * x == target
    # (z // 26) * 26 + w + yy == target, if z%26 == w - xx
    # z = z1 + z2
    # z1 == (target - w - yy), if z2 == w - xx
    # z = (target - yy - xx), if (target - w - yy) % 26 == 0

    # (z // 26) == target, if z%26 != w - xx
    # z = target*26 + d, if d != w - xx

def solve26(w, xx, yy, target):
    if (target - w - yy) % 26 == 0:
        yield target - xx - yy
    else:
        for d in range(26):
            if d != w - xx:
                yield target*26 + d



def last_vm(z, w):
    x = 0 if z % 26 - 11 == w else 1
    return (z // 26) * (25 * x + 1) + (w + 5) * x

cyz = [
    [12, 7, 1],
    [12, 8, 1],
    [13, 2, 1],
    [12, 11, 1],
    [-3, 6, 26],
]

def vm4(z, w):
    return vm(z, w, -3, 6, 26)


def vm5(z, w):
    return vm(z, w, 10, 12, 1)


def vm6(z, w):
    return vm(z, w, 14, 14, 1)


def vm7(z, w):
    return vm(z, w, -16, 13, 26)


def vm8(z, w):
    return vm(z, w, 12, 15, 1)


def vm9(z, w):
    return vm(z, w, -8, 10, 26)


def vm10(z, w):
    return vm(z, w, -12, 6, 26)


def vm11(z, w):
    return vm(z, w, -7, 10, 26)


def vm12(z, w):
    return vm(z, w, -6, 8, 26)


def vm13(z, w):
    return vm(z, w, -11, 5, 26)


vms2 = [vm0, vm1, vm2, vm3, vm4, vm5, vm6, vm7, vm8, vm9, vm10, vm11, vm12, vm13, ]
