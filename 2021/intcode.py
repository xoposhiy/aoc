def run(program, inp):
    program = program.copy()
    ip = 0
    inp_index = 0
    output = ''

    def param(i):
        mode = modes // int(10 ** (i - 1)) % 10
        if mode == 0:
            return program[program[ip + i]]
        else:
            return program[ip + i]

    while ip < len(program):
        modes = program[ip] // 100
        opcode = program[ip] % 100
        if opcode == 99:
            break
        if opcode == 1:
            program[program[ip + 3]] = param(1) + param(2)
            ip += 4
        elif opcode == 2:
            program[program[ip + 3]] = param(1) * param(2)
            ip += 4
        elif opcode == 3:
            program[program[ip + 1]] = inp[inp_index]
            inp_index += 1
            ip += 2
        elif opcode == 4:
            v = param(1)
            output = output + chr(v)
            ip += 2
        elif opcode == 5:  # jump-if-true
            if param(1) != 0:
                ip = param(2)
            else:
                ip += 3
        elif opcode == 6:  # jump-if-false
            if param(1) == 0:
                ip = param(2)
            else:
                ip += 3
        elif opcode == 7:  # less-than
            program[program[ip + 3]] = 1 if param(1) < param(2) else 0
            ip += 4
        elif opcode == 8:  # equals
            program[program[ip + 3]] = 1 if param(1) == param(2) else 0
            ip += 4
        else:
            print("Unknown opcode: ", opcode)
            return
    return program, output
