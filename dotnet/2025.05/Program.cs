using System.Numerics;
using System.Runtime.CompilerServices;

var lines = Common.ReadInputLinesDefault();

var ranges = new List<NumberRange>();
var ok = new HashSet<BigInteger>();
var okIngredients = new List<BigInteger>();
foreach (var line in lines)
{
    if (line.Contains("-"))
    {
        var fromto = line.Split("-").Select(long.Parse).ToList();
        ranges.Add(new NumberRange(fromto[0], fromto[1]));
    }
    else if (line == "")
    {
        continue;
    }
    else
    {
        var ingredient = long.Parse(line);

        if (ranges.Where(r => r.IsBetween(ingredient)).Count() > 0)
        {
            okIngredients.Add(ingredient);
        }
    }
}
var sum1 = okIngredients.Count();
Console.WriteLine(sum1);


var sum2 = ranges.Select(r =>
{
    ranges.Where(r2 => r2 != r).ToList().ForEach(r2 => r2.Adjust(r));
    return r.RangeCount();
}).Sum();
Console.WriteLine(sum2);



class NumberRange(long from, long to)
{
    public long From = from;
    public long To = to;
    public bool IsNothing = false;
    public bool IsBetween(long input)
    {
        return input >= From && input <= To;
    }
    public long RangeCount()
    {
        if (IsNothing) return 0;
        return long.Abs(From - To) + 1;
    }
    public IEnumerable<long> Iter()
    {
        if (IsNothing) yield break;
        for (var i = From; i <= To; i++)
        {
            yield return i;
        }
    }
    public bool IsOverlapping(NumberRange range)
    {
        return range.From >= From && range.From <= To;
    }
    public bool IsWithin(long nr) => nr >= From && nr <= To;
    public NumberRange merge(NumberRange range)
    {
        var from = From < range.From ? From : range.From;
        var to = To > range.To ? To : range.To;
        return new NumberRange(from, to);
    }

    public void Adjust(NumberRange range)
    {
        var isToWithin = IsWithin(range.To);
        var isFromWithin = IsWithin(range.From);
        if (isToWithin && isFromWithin)
        {
            range.IsNothing = true;
        }
        if (isToWithin)
        {
            range.To = From - 1;
        }
        if (isFromWithin)
        {
            range.From = To + 1;
        }
    }
}