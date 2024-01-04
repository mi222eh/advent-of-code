
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
        line.StepCount = 1000000;
    }
}

for (var i = 0; i < space.First().Count(); i++)
{
    var column = space.Select(line => line[i]).ToList();
    if (column.All(x => x is EmptySpace))
    {
        foreach (var line in space)
        {
            line[i].StepCount = 1000000;
        }
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
    public int StepCount { get; set; } = 1;
};
public class Space() : List<SpaceLine>();

public interface Spot
{
    public int StepCount { get; set; }
}


public class GalaxyPair(Galaxy galaxy1, Galaxy galaxy2, Space space)
{
    public Galaxy Galaxy1 { get; init; } = galaxy1;
    public Galaxy Galaxy2 { get; init; } = galaxy2;

    public uint Distance => space.GetDistance(Galaxy1, Galaxy2);
};
public class Galaxy() : Spot
{
    public int StepCount { get; set; } = 1;
};
public class EmptySpace() : Spot
{
    public int StepCount { get; set; } = 1;
};

public static class Extensions
{
    public static List<Galaxy> GetGalaxies(this Space space) => space.SelectMany(line => line).OfType<Galaxy>().ToList();
    public static (int x, int y) GetCoordinatesOfSpace(this Space space, Spot spaceToFind)
    {
        for (var y = 0; y < space.Count; y++)
        {
            var line = space[y];
            for (var x = 0; x < line.Count; x++)
            {
                var spot = line[x];
                if (spot == spaceToFind)
                {
                    var ySteps = space.Take(y).Select(l => l.StepCount).Sum();
                    var xSteps = line.Take(x).Select(s => s.StepCount).Sum();
                    return (xSteps, ySteps);
                }
            }
        }
        throw new Exception("Space not found");
    }
    public static uint GetDistance(this Space space, Spot space1, Spot space2)
    {
        var (x1, y1) = space.GetCoordinatesOfSpace(space1);
        var (x2, y2) = space.GetCoordinatesOfSpace(space2);
        return (uint)(Math.Abs(x1 - x2) + Math.Abs(y1 - y2));
    }
}