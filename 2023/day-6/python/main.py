
from pathlib import Path
from os import getcwd
import functools

class Race():
    def __init__(self, time:int, distance:int):
        self.distance = distance
        self.time = time 
def get_distance_with_boat(wait_time:int, total_time:int) -> int:
    total_time = total_time - wait_time
    distance = wait_time * total_time
    return distance

def get_wait_times_of_race(race:Race):
    wait_times:list[int] = []
    for n in range(0, race.time):
        distance = get_distance_with_boat(n, race.time)
        if distance > race.distance:
            wait_times.append(n)
    return wait_times



races:list[Race] = [];

time_line, distance_line = Path(__file__).parent.joinpath("input.txt").read_text().splitlines()

for n in range(0, len(time_line.split(":")[1].split())):
    time_str = time_line.split(":")[1].split()[n]
    distance_str = distance_line.split(":")[1].split()[n]
    races.append(Race(int(time_str), int(distance_str)))

single_time= int(functools.reduce(lambda a,b : a+b, time_line.split(":")[1].split()))
single_distance= int(functools.reduce(lambda a,b : a+b, distance_line.split(":")[1].split()))

single_race = Race(single_time, single_distance)
single_race_win_times = get_wait_times_of_race(single_race)

times_of_races = [ get_wait_times_of_race(race) for race in races]

result = functools.reduce(lambda a,b: a * b, [len(times) for times in times_of_races])

print(result)
print(len(single_race_win_times))