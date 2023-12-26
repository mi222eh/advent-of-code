import pathlib

lines = open(__file__ + "/../output.txt", "r").read().splitlines()

nrs = map(lambda n: int(n), lines)

total = sum(nrs)
print(total)