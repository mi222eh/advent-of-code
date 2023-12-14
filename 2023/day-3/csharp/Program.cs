
var result = 0;
var lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "input.txt")).ToList();

var numberPartList = lines.Select((line, i) => line.GetNumbersAtLine(i)).Aggregate((a, b) => { a.AddRange(b); return a; });

lines.Select((line, y) => (line, y)).ToList().ForEach(itemY =>
{
    var (line, y) = itemY;
    var xList = line.GetGearIndexes();
    xList.ForEach(x =>
    {
        var rangeY = Enumerable.Range(Helper.EnsurePositive(y - 1), 3);
        var rangeX = Enumerable.Range(Helper.EnsurePositive(x - 1), 3);
        var gearNrs = new HashSet<NumberPart>();

        foreach (var posX in rangeX)
        {
            foreach (var posY in rangeY)
            {
                var nr = numberPartList.Find(x => x.IsWithin(posX, posY));
                if (nr is NumberPart nrPart)
                {
                    var added = gearNrs.Add(nrPart);

                }
            }
        }
        if (gearNrs.Count() > 1)
        {
            var nrs = gearNrs.Select(x => x.Number);
            Console.WriteLine($"Gear @ {y + 1}:{x + 1} has the Numbers {nrs.First()}:{nrs.Last()}");
            result += nrs.First() * nrs.Last();
        }
    });
});

Console.WriteLine($"Result is {result}");



static class Helper
{
    public static int EnsurePositive(int nr) => nr < 0 ? 0 : nr;

    public static List<int> GetGearIndexes(this List<string> lines, int y) => lines[y].GetGearIndexes();
    public static List<int> GetGearIndexes(this string line) => line.ToCharArray().Select((c, i) => c == '*' ? i : -1).Where(i => i > -1).ToList();

    public static List<NumberPart> GetNumbersAtLine(this string line, int y)
    {
        var numbers = new List<NumberPart>();
        var currentIndex = 0;
        var list = line.ToCharArray().Cast<char?>() ?? new List<char?>();
        while (true)
        {
            var c = list.ElementAtOrDefault(currentIndex);
            if (!c.HasValue) break;
            if (char.IsDigit(c.Value))
            {
                var currentNr = c.ToString() ?? "";
                var currentNrIndex = currentIndex;
                while (true)
                {
                    currentNrIndex++;
                    var nextC = list.ElementAtOrDefault(currentNrIndex);
                    if (nextC.HasValue && char.IsDigit(nextC.Value))
                        currentNr = $"{currentNr}{nextC}";
                    else
                        break;
                }
                numbers.Add(new NumberPart(int.Parse(currentNr), currentIndex, y));
                currentIndex = currentNrIndex;
            }
            currentIndex++;
        }

        return numbers;
    }
}



record NumberPart(int Number, int x, int y)
{
    public int NumberSize { get => Number.ToString().Length; }
    public int EndX { get => x + NumberSize - 1; }
    public bool IsWithin(int inX, int inY) => inX >= x && inX <= EndX && y == inY;
}

