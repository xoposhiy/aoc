from enum import Enum

def readInts(fn):
  with open(fn) as f:
    return list(map(int, f.readlines()))

class VmStatus(Enum):
    NOT_STARTED = 0
    WAIT_INPUT = 1
    HAVE_OUTPUT = 2
    HALTED = 3

class Vm:
    def __init__(self, mem, base=0, ip=0, status=VmStatus.NOT_STARTED, output=None):
        self.mem = mem
        self.base = base
        self.ip = ip
        self.status = status
        self.output = output

def run_to_end(program, input, logging=False):
    output = []
    vm = Vm(program)
    while True:
        vm = run(vm, input, logging)
        if vm.status == VmStatus.HAVE_OUTPUT:
            output.append(vm.output)
        elif vm.status == VmStatus.HALTED:
            return output
        else:
            raise Exception('{} {}'.format(vm.status, vm))

def run(vm, input, logging=False):
    base = vm.base
    ip = vm.ip
    p = vm.mem

    def get_addr(i, modes):
        mode = modes//int(10**(i-1))%10
        addr = 0
        if mode == 0:
            return p[ip+i]
        elif mode == 1:
            return ip+i
        else:
            return base+p[ip+i]

    def setmem(i, modes, value):
        addr = get_addr(i, modes)
        while addr >= len(p):
            p.append(0)
        p[addr] = value

    def param(i, modes):
        addr = get_addr(i, modes)
        while addr >= len(p):
            p.append(0)
        return p[addr]

    while ip < len(p):
        modes = p[ip]//100
        opcode = p[ip]%100

        def formatArg(mode, i, val):
            res = str(p[ip+i])
            if mode == 0:
                res = "*"+res + "(" + str(param(i, modes))+")"
            if mode == 2:
                res = "@"+res + "(" + str(param(i, modes))+")"
            return res
            
        def log(name, n):
            if logging:
                parts = [str(ip), name]
                for i in range(n):
                    mode = modes//int(10**i)%10
                    parts.append(formatArg(mode, i+1, param(i+1, modes)))
                print(" ".join(parts))

        if opcode == 99: 
            break
        if opcode == 1:
            log("add", 3)
            setmem(3, modes, param(1, modes) + param(2, modes))
            ip+=4
        elif opcode == 2:
            log("mul", 3)
            setmem(3, modes, param(1, modes) * param(2, modes))
            ip+=4
        elif opcode == 3:
            log("inp", 1)
            if len(input) == 0:
                return Vm(p, base, ip, VmStatus.WAIT_INPUT)
            setmem(1, modes, input.pop(0))
            ip+=2
        elif opcode == 4:
            log("out", 1)
            v = param(1, modes)
            ip+=2
            return Vm(p, base, ip, VmStatus.HAVE_OUTPUT, v)
        elif opcode == 5: #jump-if-true
            log("ifT", 2)
            if param(1, modes) != 0:
                ip = param(2, modes)
            else:
                ip+=3
        elif opcode == 6: #jump-if-false
            log("ifF", 2)
            if param(1, modes) == 0:
                ip = param(2, modes)
            else:
                ip+=3
        elif opcode == 7: #less-than
            log("lt ", 3)
            setmem(3, modes, 1 if param(1, modes) < param(2, modes) else 0)
            ip+=4
        elif opcode == 8: #equals
            log("eq ", 3)
            setmem(3, modes, 1 if param(1, modes) == param(2, modes) else 0)
            ip+=4
        elif opcode == 9: #set-rel-base
            log("rel-base ", 1)
            base = base + param(1, modes)
            ip+=2
        else:
            raise Exception("Unknown opcode: {}".format(opcode))
    return Vm(p, base, ip, VmStatus.HALTED)
