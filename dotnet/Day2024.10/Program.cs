// See https://aka.ms/new-console-template for more information
var map = new Map();

map.Init(Common.ReadInputLinesDefault().ToList());

var sum1 = map.GetCoordsByNr(0).Select(c =>
{
    var score = map.TestTrails(0, c).Distinct();
    return score.Count();
}).Sum();
Console.WriteLine(sum1.ToString());

var sum2 = map.GetCoordsByNr(0).Select(c =>
{
    var score = map.TestTrails(0, c);
    return score.Count();
}).Sum();
Console.WriteLine(sum2.ToString());

class Map
{
    public List<List<int>> Topology = new();
    public void Init(List<string> lines)
    {
        Topology = lines.Select(l => l.Select(c => int.Parse(c.ToString())).ToList()).ToList();
    }

    public List<Coord> TestTrails(int step, Coord coord)
    {
        if (step == 9)
            return [coord];

        step++;
        var valid = GetCoordsByNrNextToCoords(step, coord);
        if (valid.Count == 0) return [];
        return valid.SelectMany((c) => TestTrails(step, c)).ToList();
    }

    public List<Coord> GetCoordsByNr(int nr)
    {
        List<Coord> coords = [];

        for (int y = 0; y < Topology.Count; y++)
        {
            var line = Topology[y];
            for (int x = 0; x < line.Count; x++)
            {
                var nrInTop = line[x];
                if (nr == nrInTop)
                    coords.Add(new Coord(x, y));
            }
        }
        return coords;
    }

    public List<Coord> GetCoordsByNrNextToCoords(int nr, Coord coord) => GetCoordsByNr(nr).Where(x => x.IsNextTo(coord)).ToList();
}

record Coord(int X, int Y)
{
    public bool IsNextTo(Coord coord)
    {
        return new Coord(coord.X - 1, coord.Y) == this
        || new Coord(coord.X, coord.Y - 1) == this
        || new Coord(coord.X + 1, coord.Y) == this
        || new Coord(coord.X, coord.Y + 1) == this;
    }
};
