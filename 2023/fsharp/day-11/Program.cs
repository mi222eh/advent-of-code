
var inputLines = Common.ReadInputLines("input.txt");

var space = new EntireSpace(inputLines
    .Select((line, y) =>
        new SpaceLine(line.ToCharArray().Select((c, x) => c switch
            {
                '.' => new EmptySpace(x, y),
                '#' => new Galaxy(x, y) as Space,
                _ => throw new Exception("Unknown space type")
            }
        ).ToList(), y)
    ).ToList());

foreach (var line in space.Lines)
{
    if (line.Spaces.All(x => x is EmptySpace))
    {
        space.Lines.Insert(line.Y, new SpaceLine(new List<Space>(), line.Y));

    }
}

Console.WriteLine($"Created Space");
Console.WriteLine($"Galaxies: {space.Galaxies.Count}");



var allGalaxies = space.Galaxies;
var uniquePairs = allGalaxies
    .SelectMany((g1, i) => allGalaxies.Skip(i + 1), (g1, g2) => new GalaxyPair(g1, g2))
    .Distinct()
    .ToList();
var result = uniquePairs.Sum(cg => cg.Distance);

Console.WriteLine($"Result 1: {result}");
var linesToWrite = uniquePairs.Select(cg => $"{cg.Galaxy1.X},{cg.Galaxy1.Y} - {cg.Galaxy2.X},{cg.Galaxy2.Y} is {cg.Distance}").ToList();
File.WriteAllLines("output.txt", linesToWrite);
Common.PressAnyKeyToContinue();


public record EntireSpace(List<SpaceLine> Lines)
{
    public List<Galaxy> Galaxies => Lines.SelectMany(sl => sl.Spaces).OfType<Galaxy>().ToList();
    public List<EmptySpace> EmptySpaces => Lines.SelectMany(sl => sl.Spaces).OfType<EmptySpace>().ToList();
    public List<Galaxy> GetOtherGalaxies(Galaxy galaxy) => Galaxies.Where(g => g != galaxy).ToList();
};
public record SpaceLine(List<Space> Spaces, int Y);

public interface Space { }
public class GalaxyPair
{
    public Galaxy Galaxy1 { get; init; }
    public Galaxy Galaxy2 { get; init; }
    public GalaxyPair(Galaxy galaxy1, Galaxy galaxy2)
    {
        Galaxy1 = galaxy1;
        Galaxy2 = galaxy2;
    }
    public override bool Equals(object? obj)
    {
        return obj is GalaxyPair pair && IsSamePair(pair);
    }
    private Galaxy TopLeftMost => Galaxy1.X < Galaxy2.X ? Galaxy1 : Galaxy1.Y < Galaxy2.Y ? Galaxy1 : Galaxy2;
    private Galaxy BottomRightMost => Galaxy1.X > Galaxy2.X ? Galaxy1 : Galaxy1.Y > Galaxy2.Y ? Galaxy1 : Galaxy2;
    public override int GetHashCode() => HashCode.Combine(TopLeftMost, BottomRightMost);
    public int Distance => Math.Abs(Galaxy1.X - Galaxy2.X) + Math.Abs(Galaxy1.Y - Galaxy2.Y);
    public bool IsSamePair(GalaxyPair other) =>
        (Galaxy1 == other.Galaxy1 && Galaxy2 == other.Galaxy2) ||
        (Galaxy1 == other.Galaxy2 && Galaxy2 == other.Galaxy1);
};
public record Galaxy(int X, int Y) : Space;
public record EmptySpace(int X, int Y) : Space;

public static class Helper
{
    public static bool ContainsPair(this IEnumerable<GalaxyPair> pairs, GalaxyPair pair) =>
        pairs.Where(p => p.IsSamePair(pair)).Count() > 0;
}