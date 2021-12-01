from shutil import copyfile
from os import listdir

# create solution files for the next day of AoC
nextDay = max([int(f[0:2]) for f in listdir(".") if f[0:2].isdigit() and f.endswith(".py")])+1
copyfile("00.py", f"{nextDay:02d}.py")
copyfile("00.txt", f"{nextDay:02d}.txt")

