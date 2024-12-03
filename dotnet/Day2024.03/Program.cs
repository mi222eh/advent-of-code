using System.Text.RegularExpressions;

var mul_regex = new Regex("mul\\(\\d{1,3}\\,\\d{1,3}\\)");

var input = Common.ReadInputDefault()!;

var dos = new Regex("do\\(\\)").Matches(input).Select(x => x.Index).ToList();
dos.Sort();

var donts = new Regex("don't\\(\\)").Matches(input).Select(x => x.Index).ToList();
donts.Sort();

var total = mul_regex.Matches(input).ToList()
    .Where(x => dos.FindLast(doIndex => x.Index > doIndex) >= donts.FindLast(dontIndex => x.Index > dontIndex))
    .Select(x => x.Value)
    .Select(x => x.Replace("mul(", ""))
    .Select(x => x.Replace(")", ""))
    .Select(x => x.Split(",").Select(int.Parse))
    .Select(x => x.Aggregate((nr1, nr2) => nr1 * nr2))
    .Sum();

Console.WriteLine(total);

