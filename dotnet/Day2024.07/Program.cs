using System.IO.Compression;
using System.Numerics;

var lines = Common.ReadInputLinesDefault().ToList().Select((line) =>
{
    var lst = line.Split(":");
    return new Line(long.Parse(lst[0]), lst[1].Trim().Split(" ").Select(x => x.Trim()).Select(long.Parse).ToList());
});

var totalCount = lines.Count();
var doneAmount = 0;

var totalSum = lines.AsParallel().Where(x =>
{
    var isValid = x.IsValid();
    doneAmount++;
    Console.WriteLine($"Done {doneAmount}/{totalCount}");
    return isValid;
}).Select((c) => c.Result).Aggregate((i, a) => i + a);

Console.WriteLine(lines);




enum Operator
{
    Addition = 1,
    Multiplication = 2,
    Elephant = 3
}

class Line(long result, List<long> numbers)
{
    public long Result = result;
    public List<long> Numbers = numbers;

    public bool IsValid()
    {
        var operations = Utils.GetPossibleOperators(Numbers).DistinctBy(x => string.Join("", x.Select(x => (int)x))).ToList();
        return operations.Any(x => Utils.ApplyOperations(Numbers, x) == Result);
    }

}

class Utils
{
    public static long ApplyOperation(long nr1, long nr2, Operator @operator) => @operator switch
    {
        Operator.Addition => nr1 + nr2,
        Operator.Multiplication => nr1 * nr2,
        Operator.Elephant => long.Parse($"{nr1}{nr2}"),
        _ => throw new Exception("Invalid Operator")
    };

    public static Operator GetNext(Operator current) => current switch
    {
        Operator.Addition => Operator.Multiplication,
        Operator.Multiplication => Operator.Elephant,
        Operator.Elephant => Operator.Elephant,
        _ => throw new Exception("Invalid Operator")
    };

    public static long ApplyOperations(List<long> numbers, List<Operator> operators)
    {
        if (numbers.Count == 0)
            return 0;
        long total = numbers.First();

        for (int i = 1; i < numbers.Count; i++)
        {
            var number = numbers[i];
            var op = operators[i - 1];
            total = ApplyOperation(total, number, op);
        }
        return total;
    }

    public static IEnumerable<List<Operator>> GetPossibleOperators(List<long> numbers, List<Operator>? operators = null)
    {
        if (operators is null)
            operators = Enumerable.Range(0, numbers.Count - 1).AsEnumerable().Select((i) => Operator.Addition).ToList();

        yield return operators;
        for (int i = 0; i < operators.Count; i++)
        {
            var newOne = new List<Operator>(operators);
            newOne[i] = GetNext(operators[i]);
            if (newOne[i] == operators[i])
                continue;
            foreach (var result in GetPossibleOperators(numbers, newOne))
            {
                yield return result;
            }
        }
    }

    public static IEnumerable<long> IterResults(List<long> numbers, List<Operator> operators)
    {
        yield return ApplyOperations(numbers, operators);
        for (int i = 0; i < operators.Count; i++)
        {
            var newOne = new List<Operator>(operators);
            newOne[i] = GetNext(operators[i]);
            if (newOne[i] == operators[i])
                continue;
            foreach (var result in IterResults(numbers, newOne))
            {
                yield return result;
            }
        }
    }
}