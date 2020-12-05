import functools 
import re

inp = """BFFFBBFRLR
FBFFFBBLRL
BFFFFFBRRR
FBBBFFFLRR
FFBFBFBRLL
FBBFFBBLRR
FBBFFFFRLL
BFFFBBFRLL
BBFBFBFRLL
BFFFFFBRLL
FFBFFFBRRR
BBFFBFFRLR
BBFFBBFLLL
FBFBFBFLLL
BBFFFFFLLR
FFBBBFFRLL
FFBFFBFRRR
BFFFBBFLRL
BFBBBBFLRR
BFBFFBFLLL
FFBFFBBLRL
FFBFBFFRLR
FFBFBBBLLR
FFFBBFBLLR
FFFBBBFLRL
FBFBBBFLLL
FBBFFFBLLR
BFFFBBBRLL
FFBBFBBRRL
FBFFBBBLLR
FBFBBFFRRL
FBBBBFFLLL
FFBFBFFLLR
BFFFFBFLLR
FBBBFBFLRR
FBFFFFFRRR
FFBFBBFRRL
BFFFBBBLLR
FBBFFBFRLL
BFFFFBBRLL
FBBBFFFRRL
BBFBFFFLLR
FBBFBBFRLR
FFFBFFBRRR
BFBBFBBLRL
FBBBFFBLRR
BBFFBBFLRR
FFBBFBBLLL
BFFBBBBRRL
FBBFBFBRLL
BFFFFFFRRL
BBFBFBFLLL
FBFFBFFLLR
FBFBFBFRLR
BFBFBBBRLR
BBFFBBFLLR
BFFBFBFRRR
BFBBFBFLLL
FBFBBFBRLR
FBBFBFBLRL
FBFFBFFLRR
BFFFFFFRRR
BFFFFBFRLL
FBFBFFFLLR
BFFFFBFLRL
BBFFFFFRLL
FBFBFFFLRR
BFFBFBBLRL
BFBFBBFLLR
BFBFBBBLRL
BFFBFFFRRL
BFFFFFFLLL
FFBBBBBLLR
FBBBFFFLLL
BFFFBBFRRL
FFFBFBBRLL
FFFBFFBRRL
BFBFBFBRRR
BBFFFBFLLR
BFFFBFFLLL
FBFBBFBLRR
FFBBBBFLRR
BFFBBBFLRR
BBFFBBBLLR
FFFBFBBRRL
BBFFFBBLRR
FFBFFFBRRL
FFBFFFBRLR
BFBFBFFRLL
FBFFFBFLRL
BBFFFFFRLR
BFBBFFBRLR
FBFBFBBRRR
BFBFFFFLLR
FFBFBFBLRR
FFFBBBBRRL
FBFFBBFRRR
BFFBBBBLRR
BFFBFFFLLR
BFBFFBFLLR
FBBBBBBRLR
BFBFFFFLRL
BBFFBFFLLL
FBBBBBBLLR
FFBBBBBRRL
BFBFFFFLLL
FBFBBBFRLL
BFFBBBFLRL
FBBFBBBLLL
FBFBBFBRRR
FFBBFFFRLL
BBFFBBBLRL
FBBFFFBRLL
BBFBFFBRLR
FBBBFFBLLR
BFBFFFBRRL
FBFFBBBRLL
FBFBBBBRLL
BFFFBFFRLR
FFBBFBFRLR
BFBFBFFLRR
FFFBBFFLRR
BFBFBBBLLL
BFBFBFBRLR
BFFFFBFLRR
FFBBBFBRRR
BFFFFBBLLR
BBFFBFBLRR
BFBFFFBRRR
BFFFFBBRLR
FBBBBBFRRR
FBBFBBFLLR
FFBBFBBRLL
BFBBFFBLRR
FBBBFFFLLR
BFFFBFFLRL
FBBBBFBLLR
BFBBFBFLRR
BFBBBFFLLL
BFFBFBFRLL
BFBBFBFRRL
BFFBFFBLRR
FFBFFFBLLL
FFFBFBFLRL
FBBBFBBLRR
BFFFBBBLRR
FBFFFFFRLR
BBFFBBBLRR
BFBFFBBLRR
FBFFBBBRRR
FBFFBFFLLL
BFBBBBFRLL
FBBBBBBRRL
BFFBBBBLRL
FFBFBBFLRR
FFBBBFBLLL
FBBFFFFRLR
BBFBFBFRRR
FBFBFFBLRL
FFBBFBBLRL
FBFBBFFLLL
BFFFBFBLRR
FBFBFFBLLR
FBBBBBFLRL
FFBFBFFRLL
BBFFBBFRRR
FFFBBFFLLR
BFBBBFBLRR
FFBBFFBLRL
BFFBBBFRRL
BFBBBBFRRL
BBFFBFFRRL
FFBFFBBRRR
BFFBFBFLRR
FBFBFFBRRR
FBBFBBFLRL
FBFFFFBLRL
FBFFFBBLLL
FFBFFBFRLR
FBFFFBBRRL
BFBBBFBLLL
BFBFBFFLRL
BFBFFBFRRR
BBFFBBBRLL
BBFFFBFLRL
BBFFBFFRLL
FBBFFBFLLL
FBBBBFFLLR
FFBFFFFLLR
FFBBBBFLLR
BFBBBBBRRR
FFBBFBFRRR
FFBFFBFLRL
FBFBBFFRRR
BFBBBFFRRR
FBBFBBFRLL
FBBFBBBRLR
BFFFFFBRLR
BFFBFBBRLL
FBFFFBBLRR
FFBFFFFRLL
FBBBBFBRRL
FBBBBFBRLR
BFBBBBFLLL
FBBBBBFRLL
FBFBFBFLRL
FFBFBBFLLL
FBBBBFFRRL
BFFBFBFLLL
FFFBBBFLLL
FBFBBBBLLL
BFBBBBFRLR
BBFBFFFRRR
FBBBBBBLRL
BFFFBFBLRL
FFBBFBFLLR
BFBBBFBLRL
FBBBBBBRLL
FFBBFFFRRR
BFFFFBBRRR
FBFFBBBLLL
BBFBFFFLRL
FFBFFFBLRR
FFBBBFBLLR
BFFBBFFRRR
FBBFBFFLRL
FFBBBBBLRR
FBBBFBBLLL
BBFBFBFRLR
FBFBFBBLRR
FBBBFBBRLR
FBFBFBBLLR
FFFBBFBRRR
FBBBFFBRLL
FFBBBBFLRL
BBFFBFFRRR
FBFBFBFLLR
FBBBFBFRLR
BFFBBBFRRR
FFFBFBBLLR
BBFBFBBLRR
FBFFBBFRLR
FFBBBFFLLR
FFBBFBFLRL
BFFBFFFLLL
BFBFFBFRRL
FBFBBBBRRL
BFBBBBFLLR
FBBFFBBRLL
BFFBBFBRLR
BFBFFFBLRR
FFBBFFBLRR
FBBFFFFRRL
BFBBFBBLLR
FBFFFBFRLR
BFBBBBFLRL
BFBBBBBRLL
FFBBBBFRLR
BBFBFFBLLR
FBBFFFBLRR
FBFFFFFLLL
FBBFFBFRLR
FBFFBBBRRL
FFBFBFBRRL
FFBBBBFLLL
FFBFBBBRLR
FBBFBFBRLR
BBFFFBBLLR
FBBFFBBLLL
FFBBFFFLLL
FBBBBBFLRR
FFBBFFFLRR
BFFBFFBLLR
FBFFBFBRRR
FFFBFFBRLR
FBFFFFFLRR
BFFFBFBRLL
FBFBBFBRRL
FFBFBBBLRL
BFFFBBFLLR
FBBFFBBRRR
BFBBFBFRLL
FBFFFFBRLL
BBFFFFBLRR
BFFBFBFLLR
FFBFBBBRRL
FBBFFFBRLR
FFBBBBFRRR
BFBBFFBLLR
BFBFFFBLLL
BFFFBBBRRR
FBFBBBFRRR
BFBFFBBRRL
FBFBBFBLLR
BFFFBBBRLR
BFFFFBFLLL
BFFBFFBRLR
FBBFBFBRRR
FFBBFBBRRR
FBFFFBBLLR
BFBBFBBRLR
FFBBBBBRLR
FBFFBBFRRL
BFFBBBFLLR
FBBBFFBLLL
BFBFBFFRRR
BFBFBBFRRR
FBBFFBBLLR
BFFFFFFLLR
FBBBBFFRLR
FBBBBBBLLL
BFBBBBFRRR
BFBBBFFRLR
FFBFFBBRLL
FFBBFBFLLL
BFBFFFBLRL
FFFBBBFLRR
FBBFBFFRRL
FFFBFBFLLL
BFFBBFBLLL
BBFFFFBLLL
BFBBBBBRRL
FBBFBFBLRR
FBFFBFBRLL
BBFBFBBLLL
FFBBBFBLRR
FBBBFFFRRR
BFFBBBBLLL
FBFBFBFRRR
BFFFFFBLLL
BFBFBFBLLR
BBFFFFBRRL
FBBBFBFLLL
FFBFFFFLLL
FBFBFFBLLL
FBBBBBBRRR
BFBFFBFLRR
FFBFBFBLLL
FFBFFBFRRL
BFBFBBFRRL
FFBBFBBLLR
FBBBBBFLLR
BFFBFFFRLL
BBFFFBBLLL
FBFFFFFLLR
FFBBBFFRRR
FBFFFBFRRR
FFFBBBBLRL
FBFBFBBRRL
FBBFBFBLLL
BFBFFFFLRR
BBFFBFBLRL
FFFBFBFRLR
BFFBBFFLRL
FFFBBFFRRL
BFBFBFBRRL
FFBBBFFLLL
BBFFFBFLRR
FFFBBBBLLR
FFBBBFFRLR
FFBFBBFLRL
FFBBBFBLRL
BFBFFFFRRL
FBFBBFFLRR
FBBBBBBLRR
FBFBBBBRRR
FFBFBFFLRR
FBBFBFFRLR
BFFBBBBRRR
BFFFBFBLLL
BFFBBFFLLL
BFFFFBBLRL
FFBBBBBLLL
FBBBBFFRRR
FFBBFFBRRL
BBFBBFFLLL
FFFBBFBRRL
BFBFFFFRLR
FBFFBFBLLL
FFFBBFFLRL
BBFFBFFLLR
FBBFBFFLRR
FBFFBBBRLR
FBFFFFFRRL
FFFBBFBRLL
BFFBFFFLRL
BFFBBFBLRR
FFBFBFBLRL
FBBFBFBLLR
BBFFFFBLLR
BFBFBFBLRL
FFBBBFBRRL
BBFFBFBLLR
BBFFFFBRRR
FFBFBFBRLR
BBFBFFFRLR
FBBBFBFRLL
FFBBFBFRRL
BBFBFBBLLR
FFBBFFFLRL
FFBFFBBLLL
FBFBBFFLLR
FFBFFFFLRR
BFBBFFBRLL
BFBBFBBLLL
BBFBFFFRRL
FBBBFBBRRL
BBFBFBBRLR
BBFBFFBRRL
FBFBBBFLRL
BFBFBFFRRL
FFBBBBFRRL
BFFFBBFRRR
FFFBBBBRLL
FBBBFFBRLR
FBBFBBBRLL
BFBFFBBLRL
FFFBBBFLLR
BFBBFBBRRR
FFFBFBBLLL
FFFBBBBRLR
FBFFBFBLLR
FBFBFBBRLR
FFBBFFBRLR
BBFFFFFLLL
FFFBFFBRLL
FFBBBBBRLL
FBBFBFFLLR
BBFFFBBLRL
FBBBFBBLLR
BFFFFFFLRL
FFFBFBFRRR
BFFFFFBLRL
BFBBFBFLRL
BFBBFFBRRL
FBFBBFFRLL
FFBFBFFRRL
BFFFBBFLLL
BBFFBFBRLR
BFFBFBBLLR
FBFBBBBLRL
FBFFBFFRRR
FBFFFBBRRR
FBBFFBBLRL
FBFBBBBRLR
BFBBFFFRRR
BFFBBFBRRL
FBBFBFFRLL
FFBBFFFRLR
FFFBFBFLRR
FFBBBBFRLL
FBBFBBBLRL
BBFFBBFRLR
BBFFFBFLLL
BFFFFBFRRL
BFFBBFBLLR
FFBFFFBLLR
FFFBFBBLRR
BFBFFBFLRL
BFFBBFBRLL
FBFFBBFRLL
FFBBFBFLRR
BBFFBFBRLL
FBBBFFBRRL
FBFBBBFLLR
FFBBFFFLLR
FBFFBBFLRR
BFBFBFBLLL
BBFBFFBLLL
BFFFBBFLRR
BFBBBFBRLL
BFBFBBFRLL
BBFBFFFLLL
FBFFFFBLRR
BFFFBFBLLR
FFBBFFBLLL
FBBBFFFLRL
BFFBFFBRRL
FBBFBFBRRL
FBBBBBFRLR
FBBBBFFLRL
FBBBFBFRRL
BFFFBBBRRL
FBBFBFFRRR
BFFFFBFRLR
BBFBFFBLRR
BFBBBBBRLR
FBFFFBBRLR
FBFFBBFLRL
FBFFFFBLLR
BFFBFBFRRL
FFBFBBBLLL
FBBFFBFLRR
BFBFBFBLRR
FFFBFBFRRL
FFBBBBBLRL
BFFBFFFRRR
FFBFBFBLLR
FFBFFBBRRL
BFFBFFBRLL
BBFFBFBLLL
BFBBFBBRRL
FFBFFBBRLR
BFFFBFFRRR
BFBBFFBLLL
FBFFFBFLLR
BFBFFFBLLR
FFBBBFFLRL
FFBFBBFRRR
BFBBFFFRLR
BFFFBFFLLR
FBBFBBFRRL
FBBFFBFLRL
BFBBFBFLLR
FFBBBFBRLR
FBFBBFFLRL
FBFBFBFRRL
FBBBFBFLRL
FBFBBFFRLR
FFBFFBFLRR
BFFBFBBRRL
BFFBBBBRLL
BFBFBBBRRL
BFFFFBBRRL
BFFFFBBLLL
FBFFFBFLRR
BFFFBBBLRL
BBFFBBFLRL
FFBBFBFRLL
FBBBFFFRLR
FBFFBFBRRL
BFBBBBBLLL
BBFFBFBRRR
FBBBFFBLRL
BFBBFFFLLL
BBFFFFFRRR
FFFBFFBLRR
FBBBFBFLLR
BBFFFBBRRR
FBFFFBFLLL
BBFFFBFRLR
BFFBFBFRLR
BFBBBFFRRL
BFBFBBBRLL
BFFBBFBLRL
FFBFBBFLLR
FBBFFFBLLL
BBFBFBFLRL
FFFBFBBRRR
BBFFFFBRLL
BFFBFFBLLL
BFFFBBBLLL
FFFBBFBLRR
FBBFBBBLRR
FBFFFBBRLL
FFBFFFBLRL
BBFFFBFRLL
BFBFBFFRLR
FBFBFBBRLL
FBBBFFBRRR
FBFBFFFRLR
BFBBBFBLLR
FBFFFBFRRL
BFBBFFFLRL
BFFBBFBRRR
BBFBFFFRLL
BFBBBFFLLR
FBFFBBBLRR
BFFBBBFRLL
BFBBFFFRRL
FFFBBBBLRR
BBFFBBBRLR
FBFBBBBLLR
BFFFBFBRRR
FBFBFFFLLL
FFFBBBFRRR
FFBFFBFLLL
BBFFBBBRRR
FFBFBBBRLL
FBBBBFFLRR
FBBFFFBRRL
FBBBFBBRRR
FBFBBFBLRL
BBFFFBBRLL
FBFBFFBLRR
FFBFFFFRRL
FBBBBFBLLL
FFFBBFFRRR
BFBFBBFLRL
BFBFBBBRRR
FBFFBBFLLR
FBFBFBBLLL
BFBFBBFRLR
FFBBBFBRLL
BBFBFBFLLR
BFFFBFFRRL
FFFBBBFRLL
FBFFBFFLRL
BFFFFFBLLR
FFFBBFBRLR
BBFBFFBLRL
BFFBBFFRLL
BFBBFFFLRR
BFBBBBBLLR
BFFBBFFLLR
FFBFBFBRRR
FFBBFFBLLR
FBBBFBBRLL
BFBBFFBLRL
FBBFBBBRRL
FBBBBFBLRL
BFBBBFFLRL
BFFFFFBLRR
FBFBFFFRLL
FBFBFFBRRL
FBFFBFFRLL
BFBFBFBRLL
BFFBFBFLRL
BFBBFFBRRR
BBFFFFFLRL
FBBBFBBLRL
BFBFFBBRLL
BBFFFBBRLR
BFBFFBFRLL
FFBFFFBRLL
BFFBBBBLLR
FBFFFFFLRL
BFBBBFFRLL
BFFBFBBLLL
BBFFBBBLLL
BFFBBFFLRR
BFFBFFFRLR
FBFBFFFRRL
FBBFFBFRRL
FBBFBBBRRR
BFFFFFBRRL
BFBBFBFRRR
BBFFBFFLRL
FBFBBFBRLL
FBFBFBFLRR
BBFFBBFRLL
FFBBBFFRRL
BFFBBFFRRL
BFFBFBBRRR
BBFFFFFRRL
FFBBFBBRLR
BBFBFBBRRR
BFBFFFFRLL
BFFBBFFRLR
FFFBBFBLRL
BBFFFFFLRR
BFBFBBFLLL
BFBBBBBLRR
FFFBFBFLLR
FBFFFFBLLL
BFFBBBFLLL
BFFBBBBRLR
BFFFFFFLRR
FFFBFBBLRL
BFFFFBFRRR
FBFFBBFLLL
FBFFFBFRLL
BBFBFFBRLL
BBFFFFBLRL
BBFFBFFLRR
FBBBBFBRRR
FBBFFFFLLL
FFBBBBBRRR
BFBFFBBLLR
FBFFFFFRLL
FFBFBFFRRR
FFBFFBFLLR
BFBFBBFLRR
FBFBBBBLRR
FBBFFFFLRR
FFBFBBBLRR
FBFFFFBRLR
FBFBBBFLRR
BFBFFFFRRR
FBBFFFFLLR
BBFBFBBLRL
FFBBFFBRRR
FFBFFBBLLR
BFBFBFFLLR
FBBFBBFLRR
FBBFFFBLRL
BFFBFBBLRR
FFBBFFFRRL
FBFFBFBRLR
FBFFBFBLRL
FFBFBBFRLR
FFBFFFFLRL
BBFFFFBRLR
FBFFBFBLRR
BFFBFFBLRL
BFBFFBBRRR
FBFFFFBRRL
FBFBFBBLRL
FFFBBBBRRR
BBFFBBFRRL
FBBFFBBRLR
BFFFFFFRLR
FBBFBBFRRR
FBFBBBFRLR
BFBFFFBRLR
FBBFFBFRRR
FFBBFBBLRR
FFFBBFFRLR
FFFBFBBRLR
FBBFFFFLRL
FBFBFFBRLL
BFBBFFFLLR
FBBFBBBLLR
BFFFFFFRLL
BBFFBBBRRL
FBFBBFBLLL
BBFBFBFLRR
FFBFBFFLRL
FFBFBBFRLL
FBBBFFFRLL
BBFFBFBRRL
FBBBFBFRRR
FFBFBFFLLL
FBFFFFBRRR
BFBFFBBLLL
FFFBFBFRLL
FBBFBBFLLL
BBFFFBFRRL
FFFBBFBLLL
BBFBFBBRLL
BFBBBFBRRR
BFFFFBBLRR
BFBFFFBRLL
BFFFBFFLRR
BFFBBBFRLR
FBBFFFFRRR
FBFFBBBLRL
FFBBBFFLRR
FFBFBBBRRR
BFBBFFFRLL
BFBFBBBLLR
FFFBBFFRLL
FBFBFBFRLL
FBBBBFFRLL
FFFBBBFRLR
BFBFBBBLRR
FBFFBFFRRL
FFBFFBFRLL
FBBFFFBRRR
FBBFFBBRRL
BBFFFBFRRR
FFFBBBFRRL
FFFBBFFLLL
BFBBFBFRLR
FBBBBBFLLL
BFBFBFFLLL
FBFBFFFRRR
FBFFBFFRLR
BFBBFBBLRR
FFBBFFBRLL
BFBBFBBRLL
FBBBBBFRRL
BFFFBFBRLR
BFBBBBBLRL
BFFBFFBRRR
BBFBFBFRRL
BFBFFBFRLR
BFFBFFFLRR
FBBBBFBRLL
BBFBFFBRRR
BFFFBFBRRL
BFFBFBBRLR
FBBBBFBLRR
BFBFFBBRLR
BFBBBFBRLR
BBFFFBBRRL
FBBFBFFLLL
FFBFFFFRLR
FBFBFFBRLR
BFFFBFFRLL
FBFBBBFRRL
FBFBFFFLRL
BBFBFFFLRR
FFBFFFFRRR
BFBBBFBRRL
FFFBBBBLLL
BBFBFBBRRL
FFBFFBBLRR
FBBFFBFLLR""".split()

def getid(boardingpass):
    row = functools.reduce(lambda acc, ch: acc*2 + (1 if ch == "B" else 0), boardingpass[0:7], 0)
    col = functools.reduce(lambda acc, ch: acc*2 + (1 if ch == "R" else 0), boardingpass[7:10], 0)
    return row*8 + col

print("Test GetId", getid("FBFBBFFRLR"))

print("Part One", getid(max(inp, key=getid)))

ids = sorted(map(getid, inp));
place = max(set(range(ids[0], ids[-1]+1)) - set(ids))
print("Part Two", place)
