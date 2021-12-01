#!/usr/bin/python3

def run(p, input):
    p = list(p)
    p[1] = input // 100
    p[2] = input % 100
    ip = 0
    while ip < len(p):
        if p[ip] == 99: 
            break
        if p[ip] == 1:
            p[p[ip+3]] = p[p[ip+1]] + p[p[ip+2]]
        elif p[ip] == 2:
            p[p[ip+3]] = p[p[ip+1]] * p[p[ip+2]]
        ip+=4
    #print(p)
    return p[0]

def format(p):
    ip = 0
    while ip < len(p):
        if p[ip] == 99: 
            break
        if p[ip] == 1:
            print(ip, ": add ", p[ip+1], p[ip+2], p[ip+3])
        elif p[ip] == 2:
            print(ip, ": mul ", p[ip+1], p[ip+2], p[ip+3])
        ip+=4

p = [1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,13,1,19,1,10,19,23,1,6,23,27,1,5,27,31,1,10,31,35,2,10,35,39,1,39,5,43,2,43,6,47,2,9,47,51,1,51,5,55,1,5,55,59,2,10,59,63,1,5,63,67,1,67,10,71,2,6,71,75,2,6,75,79,1,5,79,83,2,6,83,87,2,13,87,91,1,91,6,95,2,13,95,99,1,99,5,103,2,103,10,107,1,9,107,111,1,111,6,115,1,115,2,119,1,119,10,0,99,2,14,0,0]

#run([1,0,0,0,99])
#run([2,3,0,3,99])
#run([2,4,4,5,99,0])
#run([1,1,1,4,99,5,6,0,99])
print("Part One: ", run(p, 1202))

for input in range(0, 10000):
    out = run(p, input)
    if out == 19690720:
        print("Part Two: ", input)

format(p)