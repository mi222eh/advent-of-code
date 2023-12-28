
var inputLines = Common.ReadInputLines("input.txt");



var space = new Space();
space.AddRange(
    inputLines.Select(line =>
    {
        var spaceLine = new SpaceLine();
        var rawLine = line.Select(c => c switch
        {
            '.' => new EmptySpace() as Spot,
            '#' => new Galaxy() as Spot,
            _ => throw new Exception("Invalid character")
        }).ToList();
        spaceLine.AddRange(rawLine);
        return spaceLine;
    }
).ToList());

for (var i = 0; i < space.Count; i++)
{
    var line = space[i];
    if (line.All(x => x is EmptySpace))
    {
        // foreach (var k in Enumerable.Range(0, 1000000))
        // {
            line.Extra += (1000000 - 1);
        // }
    }
}

for (var i = 0; i < space.First().Count(); i++)
{
    var column = space.Select(line => line[i]).ToList();
    if (column.All(x => x is EmptySpace))
    {
        // foreach (var k in Enumerable.Range(0, 1000000))
        // {
            foreach (var line in space)
            {
                line[i].Extra += (1000000 - 1);
            }
        // }
    }
}
var allGalaxies = space.GetGalaxies();


Console.WriteLine($"Galaxies: {allGalaxies.Count()}");

var uniquePairs = allGalaxies
    .SelectMany((g1, i) => allGalaxies.Skip(i + 1), (g1, g2) => new GalaxyPair(g1, g2, space))
    .Distinct()
    .ToList();
var result = uniquePairs.Sum(cg => cg.Distance);

Console.WriteLine($"Result 1: {result}");
// var linesToWrite = uniquePairs.Select(cg => $"{cg.Galaxy1.X},{cg.Galaxy1.Y} - {cg.Galaxy2.X},{cg.Galaxy2.Y} is {cg.Distance}").ToList();
// File.WriteAllLines("output.txt", linesToWrite);
Common.PressAnyKeyToContinue();


public class SpaceLine() : List<Spot>
{
    public int Extra { get; set; } = 0;
};
public class Space() : List<SpaceLine>();

public interface Spot
{

    public int Extra { get; set; }
}


public class GalaxyPair(Galaxy galaxy1, Galaxy galaxy2, Space space)
{
    public Galaxy Galaxy1 { get; init; } = galaxy1;
    public Galaxy Galaxy2 { get; init; } = galaxy2;

    public override bool Equals(object? obj)
    {
        return obj is GalaxyPair pair && IsSamePair(pair);
    }
    private Galaxy TopLeftMost => space.GetTopLeftMost(Galaxy1, Galaxy2);
    private Galaxy BottomRightMost => TopLeftMost == Galaxy1 ? Galaxy2 : Galaxy1;
    public override int GetHashCode() => HashCode.Combine(TopLeftMost, BottomRightMost);
    public uint Distance => space.GetDistance(Galaxy1, Galaxy2);
    public bool IsSamePair(GalaxyPair other) =>
        (Galaxy1 == other.Galaxy1 && Galaxy2 == other.Galaxy2) ||
        (Galaxy1 == other.Galaxy2 && Galaxy2 == other.Galaxy1);
};
public class Galaxy() : Spot
{
    public int Extra { get; set; } = 0;
};
public class EmptySpace() : Spot
{
    public int Extra { get; set; } = 0;
};

public static class Extensions
{
    public static List<Galaxy> GetGalaxies(this Space space) => space.SelectMany(line => line).OfType<Galaxy>().ToList();
    public static (int x, int y) GetCoordinatesOfSpace(this Space space, Spot spaceToFind)
    {
        var x = 0;
        var y = 0;
        for (var i = 0; i < space.Count; i++)
        {
            var line = space[i];
            y += line.Extra;
            for (var j = 0; j < line.Count; j++)
            {
                var spot = line[j];
                x += spot.Extra;
                if (line[j] == spaceToFind)
                    return (x, y);
                x += 1;
            }
            y += 1;
        }
        throw new Exception("Space not found");
    }
    public static uint GetDistance(this Space space, Spot space1, Spot space2)
    {
        var (x1, y1) = space.GetCoordinatesOfSpace(space1);
        var (x2, y2) = space.GetCoordinatesOfSpace(space2);
        return (uint)(Math.Abs(x1 - x2) + Math.Abs(y1 - y2));
    }

    public static Galaxy GetTopLeftMost(this Space space, Galaxy galaxy1, Galaxy galaxy2)
    {
        var (x1, y1) = space.GetCoordinatesOfSpace(galaxy1);
        var (x2, y2) = space.GetCoordinatesOfSpace(galaxy2);

        if (y1 < y2)
            return galaxy1;
        else if (y1 > y2)
            return galaxy2;
        else
        {
            if (x1 < x2)
                return galaxy1;
            else if (x1 > x2)
                return galaxy2;
            else
                throw new Exception("Galaxies are at the same position");
        }
    }
}