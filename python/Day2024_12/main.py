from pathlib import Path
from typing import Self

class Coord:
    x:int
    y:int

    def __init__(self, x:int, y:int):
        self.x = x
        self.y = y

    @classmethod
    def is_same(self, coord:Self):
        return self.x == coord.x and self.y == coord.y

    def get_around(self):
        return [
            Coord(self.x - 1, self.y),
            Coord(self.x + 1, self.y),
            Coord(self.x, self.y + 1),
            Coord(self.x, self.y - 1)
        ]

class Plot:
    value = ""
    coord: Coord
    def setValue(self, value:str):
        self.value = value
        return self
    def setCoord(self, coord:Coord):
        self.coord = coord
        return self
    def is_same(self, other:Self):
        return self.value == other.value
    

class Area:
    plots: list[Plot] = []

    def contains_plot(self, plot:Plot):
        return plot in self.plots


class Garden:
    map:list[list[Plot]] = []

    def iter_plot(self):
        for y in range(len(self.map)):
            line = self.map[y]
            for x in range(len(line)):
                plot = line[x]
                yield plot, Coord(y, x)

    def get_by_coord(self, coord:Coord):
        try:
            if (coord.x < 0 or coord.y < 0): 
                raise IndexError
            return self.map[coord.y][coord.x]
        except IndexError:
            return None

    def get_adjacent_same(self, main_plot:Plot):
        list_of_plots: list[Plot] = []
        def get_next(plot:Plot):
            list_of_plots.append(plot)
            for coord in plot.coord.get_around():
                plot = garden.get_by_coord(coord)
                if plot and plot not in list_of_plots and plot.is_same(main_plot):
                    get_next(plot)

        get_next(main_plot)
        return list_of_plots

    def init(self, input:str):
        x = 0
        y = 0
        for line in input.splitlines():
            garden_line = []
            for letter in line:
                garden_line.append(Plot().setValue(letter).setCoord(Coord(x, y)))
            garden.map.append(garden_line)
        


input = (Path(__file__).parent / "input.txt").read_text()

garden = Garden()
garden.init(input)

areas:list[Area] = []

for plot, coord in garden.iter_plot():
    used_plot = False
    
    for area in areas:
        if area.contains_plot(plot):
            used_plot = True
    
    
    plots = garden.get_adjacent_same(plot)
    area = Area()
    area.plots = plots
    areas.append(area)


sum = 0
for area in areas:
    perimiter = 0
    for main_plot in area.plots:
        for coord in main_plot.coord.get_around():
            plot = garden.get_by_coord(coord)
            if plot and plot.is_same(main_plot):
                continue
            perimiter += 1
    inner_sum = perimiter * len(area.plots)
    sum += inner_sum
print(sum)




