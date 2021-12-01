from enum import Enum

class VmStatus(Enum):
    NOT_STARTED = 0
    WAIT_INPUT = 1
    HAVE_OUTPUT = 2
    HALTED = 3

class MemLogRecord:
    def __init__(self, operation, address):
        self.operation=operation
        self.address=address

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

def run_to_n_output(n, vm, inp, logging=False):
    output = []
    while len(output)<n:
        vm = run(vm, inp, logging)
        #print(len(output))
        if vm.status == VmStatus.HAVE_OUTPUT:
            output.append(vm.output)
        elif vm.status == VmStatus.HALTED:
            return (vm, output)
        else:
            raise Exception('{} {}'.format(vm.status, vm))
    return (vm, output)


def run(vm, input, logging=False, memlog=[]):
    def logmem(addr, op):
        memlog.append(MemLogRecord(op, addr))

    def logbase(addr):
        logmem(addr, 'b')

    def logwrite(addr):
        logmem(addr, 'w')

    def logread(addr):
        logmem(addr, 'r')

    def logexec(addr):
        logmem(addr, 'x')

    base = vm.base
    ip = vm.ip
    p = vm.mem
    logbase(base)

    def get_addr(i, modes):
        mode = modes//int(10**(i-1))%10
        addr = 0
        if mode == 0:
            return p[ip+i]
        elif mode == 1:
            return ip+i
        else:
            return base+p[ip+i]

    def write(i, modes, value):
        addr = get_addr(i, modes)
        logwrite(addr)
        while addr >= len(p):
            p.append(0)
        p[addr] = value

    def read(i, modes):
        addr = get_addr(i, modes)
        logread(addr)
        while addr >= len(p):
            p.append(0)
        return p[addr]

    while ip < len(p):
        modes = p[ip]//100
        opcode = p[ip]%100

        def formatArg(mode, i, val):
            res = str(p[ip+i])
            if mode == 0:
                res = "*"+res + "(" + str(read(i, modes))+")"
            if mode == 2:
                res = "@"+res + "(" + str(read(i, modes))+")"
            return res
            
        def log(name, n):
            logexec(ip)
            for i in range(n):
                logread(ip+i+1)

            if logging:
                parts = [str(ip), name]
                for i in range(n):
                    mode = modes//int(10**i)%10
                    parts.append(formatArg(mode, i+1, read(i+1, modes)))
                print(" ".join(parts))

        if opcode == 99: 
            break
        if opcode == 1:
            log("add", 3)
            write(3, modes, read(1, modes) + read(2, modes))
            ip+=4
        elif opcode == 2:
            log("mul", 3)
            write(3, modes, read(1, modes) * read(2, modes))
            ip+=4
        elif opcode == 3:
            log("inp", 1)
            if len(input) == 0:
                return Vm(p, base, ip, VmStatus.WAIT_INPUT)
            write(1, modes, input.pop(0))
            ip+=2
        elif opcode == 4:
            log("out", 1)
            v = read(1, modes)
            ip+=2
            return Vm(p, base, ip, VmStatus.HAVE_OUTPUT, v)
        elif opcode == 5: #jump-if-true
            log("ifT", 2)
            if read(1, modes) != 0:
                ip = read(2, modes)
            else:
                ip+=3
        elif opcode == 6: #jump-if-false
            log("ifF", 2)
            if read(1, modes) == 0:
                ip = read(2, modes)
            else:
                ip+=3
        elif opcode == 7: #less-than
            log("lt ", 3)
            write(3, modes, 1 if read(1, modes) < read(2, modes) else 0)
            ip+=4
        elif opcode == 8: #equals
            log("eq ", 3)
            write(3, modes, 1 if read(1, modes) == read(2, modes) else 0)
            ip+=4
        elif opcode == 9: #set-rel-base
            log("rel-base ", 1)
            base = base + read(1, modes)
            logbase(base)
            ip+=2
        else:
            raise Exception("Unknown opcode: {}".format(opcode))
    return Vm(p, base, ip, VmStatus.HALTED)
