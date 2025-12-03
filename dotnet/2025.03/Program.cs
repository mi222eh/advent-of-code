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


var sum2 = lines.Select(line =>
{
    var indexesUsed = new List<int>();
    var gathered = new List<(int nr, int index)>();
    var intList = line.Select(c => int.Parse(c.ToString()));

    while (gathered.Count() < 12)
    {
        var next = intList.GetLargestNrTupleExceptIndexes(indexesUsed);
        gathered.Add(next);
        indexesUsed.Add(next.index);
    }

    return long.Parse(string.Join("", gathered.OrderBy(c => c.index).Select(c => c.nr.ToString())));
}).Sum();
Console.WriteLine(sum2);


static class Extensions
{
    record NrEntry(int nr, int index);
    extension(IEnumerable<int> nrs)
    {
        public (int nr, int index) GetLargestNrTupleExceptIndexes(IEnumerable<int> Indexes)
        {
            return nrs.Select((nr, i) => (nr, index: i))
                .Where((tuple) => !Indexes.Contains(tuple.index))
                .MaxBy((tuple) => tuple.nr);
        }
        public int GetLargestNrExceptIndexes(IEnumerable<int> Indexes)
        {
            return nrs.Select((nr, i) => (nr, index: i))
                .Where((tuple) => !Indexes.Contains(tuple.index))
                .MaxBy((tuple) => tuple.nr).nr;
        }
        public IEnumerable<(int nr, int index)> GetLargestNrTupleListExceptIndexes(IEnumerable<int> Indexes)
        {
            var largest = nrs.GetLargestNrExceptIndexes(Indexes);
            return nrs.Select((nr, i) => (nr, index: i))
                .Where((tuple) => !Indexes.Contains(tuple.index))
                .MaxBy((tuple) => tuple.nr);
        }
    }
}