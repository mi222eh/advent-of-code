var txt = Common.ReadInputDefault();

var rangeList = txt.Split(",").Select(Range.FromTxt);

var invalidNumList = rangeList.SelectMany(r => r.GetInvalidNrs()).ToList();

var sum = invalidNumList.Sum();
Console.WriteLine(sum);

var invalidNumListV2 = rangeList.SelectMany(r => r.GetInvalidNrsV2()).ToList();
var sumV2 = invalidNumListV2.Sum();
Console.WriteLine(sumV2);


record Range(long start, long end)
{
    public static Range FromTxt(string txt)
    {
        var splitted = txt.Split('-');
        return new Range(long.Parse(splitted[0]), long.Parse(splitted[1]));
    }
};


static class Extensions
{
    extension(long nr)
    {
        public bool IsInvalidID()
        {
            var txt = nr.ToString();
            var half = txt.Count() / 2;
            var oneHalf = txt.Substring(0, half);
            var secondHalf = txt.Substring(half);
            return oneHalf == secondHalf;
        }
        public bool IsInvalidIDV2()
        {
            var txt = nr.ToString();
            var half = txt.Count() / 2;
            var current = 1;
            while (current <= half)
            {
                var currentNum = txt.Substring(0, current);
                var rest = txt.Substring(current);
                if (rest.Replace(currentNum, "") == "")
                {
                    return true;
                }
                current++;
            }
            return false;
        }
    }
    extension(Range range)
    {
        public IEnumerable<long> GetInvalidNrs()
        {
            return range.IterRange().Where(nr => nr.IsInvalidID());
        }
        public IEnumerable<long> GetInvalidNrsV2()
        {
            return range.IterRange().Where(nr => nr.IsInvalidIDV2());
        }

        public IEnumerable<long> IterRange()
        {
            var current = range.start;
            while (current <= range.end)
            {
                yield return current;
                current++;
            }
        }
    }
}