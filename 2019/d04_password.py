def count(password, start, end, i, last, first, hasDouble):
    n = len(password)
    if i == n:
        return 1 if hasDouble else 0
    c = 0
    upper = end[i] if last else 9
    if i == 0:
        password[i] = start[i]
    elif first:
        password[i] = max(start[i], password[i-1])
    else:
        password[i] = password[i-1]
    while password[i] <= upper:
        c += count(
            password, start, end,
            i+1, 
            last and password[i] == end[i], 
            first and password[i] == start[i],
            hasDouble or i > 0 and password[i] == password[i-1])
        password[i]+=1
    return c

def count2(password, start, end, i, last, first, hasDouble, repeatsAtEnd):
    n = len(password)
    if i == n:
        if hasDouble or repeatsAtEnd == 2:
            return 1 
        else:
            return 0
    c = 0
    upper = end[i] if last else 9
    if i == 0:
        password[i] = start[i]
    elif first:
        password[i] = max(start[i], password[i-1])
    else:
        password[i] = password[i-1]
    while password[i] <= upper:
        c += count2(
            password, start, end,
            i+1, 
            last and password[i] == end[i], 
            first and password[i] == start[i],
            hasDouble or repeatsAtEnd==2 and password[i] != password[i-1],
            repeatsAtEnd+1 if (i>0 and password[i] == password[i-1]) else 1)
        password[i]+=1
    return c

start = [2,4,6,5,4,0]
end = [7,8,7,4,1,9]

print("Part One:", count(start.copy(), start, end, 0, True, True, False))
print("Part Two:", count2(start.copy(), start, end, 0, True, True, False, 0))
    