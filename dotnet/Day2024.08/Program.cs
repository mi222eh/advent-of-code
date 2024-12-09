// See https://aka.ms/new-console-template for more information

var map = new Map
{
    Antennas = Common.ReadInputLinesDefault().Select((line, y) => line.Select((ch, x) => new Antenna
    {
        Coord = new Coord(x, y),
        Value = ch
    }).ToList()).ToList()
};

var coordList = map.IterAntennas(new(0, 0)).SelectMany((antenna) =>
{
    var matching = map.GetMatchingAntennas(antenna, antenna.Coord);
    var coords = matching.SelectMany(match =>
    {
        var delta = antenna.Coord.GetDelta(match.Coord);
        var invertedDelta = delta.Inverted;
        var first = antenna.Coord.PlusDelta(invertedDelta);
        var second = match.Coord.PlusDelta(delta);
        List<Coord> result = [first, second];
        return result;
    }).ToList();
    return coords;
}).Distinct().Where(c => map.IsInside(c)).ToList();

Console.WriteLine(coordList.Count);

// PART 2
var coordList2 = map.IterAntennas(new(0, 0)).SelectMany((antenna) =>
{
    var matching = map.GetMatchingAntennas(antenna, antenna.Coord);
    var coords = matching.SelectMany(match =>
    {
        var delta = antenna.Coord.GetDelta(match.Coord);
        var invertedDelta = delta.Inverted;
        List<Coord> listOfCoords = [];

        listOfCoords.Add(antenna.Coord);
        listOfCoords.Add(match.Coord);

        var first = antenna.Coord.PlusDelta(invertedDelta);
        listOfCoords.Add(first);

        while (map.IsInside(first))
        {
            first = first.PlusDelta(invertedDelta);
            listOfCoords.Add(first);
        }

        var second = match.Coord.PlusDelta(delta);
        listOfCoords.Add(second);

        while (map.IsInside(second))
        {
            second = second.PlusDelta(delta);
            listOfCoords.Add(second);
        }


        return listOfCoords;
    }).ToList();
    return coords;
}).Distinct().Where(map.IsInside).ToList();

Console.WriteLine(coordList2.Count);
map.Print(coordList2);


class Antenna
{
    public Coord Coord;
    public char Value;

}

class AntiNode
{
    public Coord Coord;

}

class Map
{
    public List<List<Antenna>> Antennas = new();
    public IEnumerable<Antenna> IterAntennas(Coord? from = null)
    {
        if (from is null) from = new Coord(0, 0);
        var isSkipped = false;
        foreach (var line in Antennas.Skip(from.Y))
        {
            IEnumerable<Antenna> antennas = line;
            if (!isSkipped) antennas = line.Skip(from.X);
            isSkipped = true;

            foreach (var ant in line)
            {
                if (char.IsLetterOrDigit(ant.Value))
                    yield return ant;
            }
        }
    }
    public bool IsInside(Coord coord) => coord.Y >= 0 && coord.Y < Antennas.Count && coord.X >= 0 && coord.X < Antennas.First().Count;
    public IEnumerable<Antenna> GetMatchingAntennas(Antenna antenna, Coord? from = null)
    {

        foreach (var ant in IterAntennas(from))
            if (ant.Value == antenna.Value && ant.Coord != antenna.Coord)
                yield return ant;
    }

    public void Print(List<Coord> antiNodes)
    {
        var toPrint = "";

        for (int y = 0; y < Antennas.Count; y++)
        {
            var line = Antennas[y];
            for (int x = 0; x < line.Count; x++)
            {
                var antenna = line[x];
                if (antiNodes.Contains(new(x, y)))
                {
                    toPrint += "#";
                }
                else
                {
                    toPrint += antenna.Value;
                }
            }
            toPrint += "\n";
        }
        Console.WriteLine(toPrint);
    }
}

record Coord(int X, int Y)
{
    public CoordDelta GetDelta(Coord coord) => new CoordDelta(coord.X - X, coord.Y - Y);
    public Coord PlusDelta(CoordDelta delta) => new(X + delta.X, Y + delta.Y);

};

record CoordDelta(int X, int Y)
{
    public CoordDelta Inverted => new CoordDelta(-X, -Y);
};