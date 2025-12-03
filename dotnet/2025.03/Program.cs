var lines = Common.ReadInputLinesDefault();

var sum = lines.Select(line =>
{
    var maxNr = line.Select(c => int.Parse(c.ToString())).Max();
    var index = line.IndexOf(maxNr.ToString());
    var secondMaxNr = 0;
    if (index + 1 == line.Count())
    {
        secondMaxNr = line.Substring(0, index).Select(c => int.Parse(c.ToString())).Max();
        return int.Parse(secondMaxNr.ToString() + maxNr.ToString());
    }
    secondMaxNr = line.Substring(index + 1).Select(c => int.Parse(c.ToString())).Max();
    return int.Parse(maxNr.ToString() + secondMaxNr.ToString());
}).Sum();

Console.WriteLine(sum);


var nrList = lines.Select(line =>
{
    var indexesUsed = new List<int>();
    var gathered = new List<(int nr, int index)>();
    var intList = line.Select(c => int.Parse(c.ToString()));
    var allTupleList = intList.ToTuple();

    while (gathered.Count() < 12)
    {
        var next = allTupleList.Where(i => !indexesUsed.Contains(i.index)).OrderByDescending(i => i.index).MaxBy(i =>
        {
            var tmp = new List<(int nr, int index)>(gathered)
            {
                i
            };
            return tmp.ToSum();
        });
        gathered.Add(next);
        indexesUsed.Add(next.index);
    }

    return long.Parse(string.Join("", gathered.OrderBy(c => c.index).Select(c => c.nr.ToString())));
}).ToList();

var sum2 = nrList.Sum();
Console.WriteLine(sum2);


static class Extensions
{
    extension(IEnumerable<int> nrs)
    {
        public IEnumerable<(int nr, int index)> ToTuple()
        {
            return nrs.Select((nr, i) => (nr, index: i));
        }

    }
    extension(IEnumerable<(int nr, int index)> values)
    {
        public long ToSum()
        {
            return long.Parse(string.Join("",values.OrderBy(i => i.index).Select(i => i.nr.ToString())));
        }
    }
}