using System.Linq;

var map = new Map();

var allSpots = Common.ReadInputLinesDefault().SelectMany((line, y) =>
{
    return line.Select<char, Spot>((ch, x) =>
    {
        if (ch == '@') return new RollOfPaper(x, y);
        return new Empty(x, y);
    });
}).ToList();

allSpots.ForEach(map.AddSpot);

var sum = allSpots.Where(s => s.IsPaper()).Select(s => map.CountRollsAround(s)).Where(nr => nr < 4).Count();
Console.WriteLine(sum);


var removables = map.Iter().GetRemovables(map).ToList();
var removed = new List<RollOfPaper>();
while (removables.Count() > 0)
{
    removables.ForEach(s =>
    {
        removed.Add(s);
        map.SetSpot(s.ToEmpty());
    });
    removables = map.Iter().GetRemovables(map).ToList();
}
var sum2 = removed.Count();
Console.WriteLine(sum2);

record Spot(int X, int Y);
record RollOfPaper(int X, int Y) : Spot(X, Y);
record Empty(int X, int Y) : Spot(X, Y);
class Map
{
    Dictionary<(int X, int Y), Spot> spotMap = new();

    public Spot? GetSpot(int X, int Y)
    {
        try
        {
            return spotMap[(X, Y)];
        }
        catch
        {
            return null;
        }
    }
    public void AddSpot(Spot spot)
    {
        spotMap.Add((spot.X, spot.Y), spot);
    }
    public void SetSpot(Spot spot)
    {
        spotMap.Remove((spot.X, spot.Y));
        AddSpot(spot);
    }
    public IEnumerable<Spot> Iter()
    {
        return spotMap.Values.AsEnumerable();
    }
    public List<Spot> GetSpotsAround(Spot spot)
    {
        var list = new List<Spot>();

        var otherSpot = GetSpot(spot.X - 1, spot.Y - 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X, spot.Y - 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X + 1, spot.Y - 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X - 1, spot.Y);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X + 1, spot.Y);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X - 1, spot.Y + 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X, spot.Y + 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        otherSpot = GetSpot(spot.X + 1, spot.Y + 1);
        if (otherSpot is not null)
            list.Add(otherSpot);

        return list;
    }
    public int CountRollsAround(Spot spot)
    {
        return GetSpotsAround(spot).Where(s => s.IsPaper()).Count();
    }
}

static class Extensions
{
    extension(Spot spot)
    {
        public bool IsPaper()
        {
            return spot is RollOfPaper;
        }
    }
    extension(RollOfPaper rollOfPaper)
    {
        public Empty ToEmpty()
        {
            return new Empty(rollOfPaper.X, rollOfPaper.Y);
        }
    }
    extension(IEnumerable<Spot> spots)
    {
        public IEnumerable<RollOfPaper> GetRemovables(Map map)
        {
            return spots.Where(s => s.IsPaper()).Select(s => (RollOfPaper)s).Where(s => map.CountRollsAround(s) < 4);
        }
    }
}
