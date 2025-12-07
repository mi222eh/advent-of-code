// See https://aka.ms/new-console-template for more information

var problems = new Dictionary<int, MathLine>();

Common.ReadInputLinesDefault().ToList().ForEach(line =>
{
    foreach (var tuple in Parser.IterLine(line))
    {
        if (!problems.ContainsKey(tuple.index))
        {
            problems.Add(tuple.index, new MathLine());
        }
        var mathLine = problems[tuple.index];
        if (long.TryParse(tuple.part, out var nr))
        {
            mathLine.numbers.Add(nr);
        }
        if (tuple.part == "*")
        {
            mathLine.@operator = Operator.Multi;
        }
        if (tuple.part == "+")
        {
            mathLine.@operator = Operator.Add;
        }
    }
});

var sum = problems.Values.Select(m => m.Calculate()).Sum();
Console.WriteLine(sum);



enum Operator
{
    None,
    Add,
    Multi
}
class MathLine
{
    public List<long> numbers = new();
    public Operator @operator;

    public long Calculate()
    {
        if (@operator == Operator.Add) return numbers.Sum();
        long number = 1;
        foreach (var nr in numbers)
        {
            number = number * nr;
        }
        return number;
    }
}
class Parser
{
    public static IEnumerable<(string part, int index)> IterLine(string input)
    {
        var index = 0;
        foreach (var part in input.Split(" "))
        {
            yield return (part, index);
            index++;
        }
    }
}