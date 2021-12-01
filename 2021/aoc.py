import sys
import os


def read_lines():
    fn = os.path.basename(sys.argv[0])[0:2]
    fh = open(fn + ".txt", "r")
    try:
        return fh.read().splitlines()
    finally:
        fh.close()


def read_ints():
    return list(map(int, read_lines()))
