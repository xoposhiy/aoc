def vm0(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 12
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 7
    y *= x
    z += y
    return z


def vm1(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 12
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 8
    y *= x
    z += y
    return z


def vm2(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 13
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 2
    y *= x
    z += y
    return z


def vm3(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 12
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 11
    y *= x
    z += y
    return z


def vm4(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -3
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 6
    y *= x
    z += y
    return z


def vm5(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 10
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 12
    y *= x
    z += y
    return z


def vm6(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 14
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 14
    y *= x
    z += y
    return z


def vm7(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -16
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 13
    y *= x
    z += y
    return z


def vm8(z, w):
    x = 0
    x += z
    x %= 26
    z //= 1
    x += 12
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 15
    y *= x
    z += y
    return z


def vm9(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -8
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 10
    y *= x
    z += y
    return z


def vm10(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -12
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 6
    y *= x
    z += y
    return z


def vm11(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -7
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 10
    y *= x
    z += y
    return z


def vm12(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -6
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 8
    y *= x
    z += y
    return z


def vm13(z, w):
    x = 0
    x += z
    x %= 26
    z //= 26
    x += -11
    x = 1 if x == w else 0
    x = 1 if x == 0 else 0
    y = 0
    y += 25
    y *= x
    y += 1
    z *= y
    y = 0
    y += w
    y += 5
    y *= x
    z += y
    return z


vms = [vm0, vm1, vm2, vm3, vm4, vm5, vm6, vm7, vm8, vm9, vm10, vm11, vm12, vm13, ]
