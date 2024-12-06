

List<NumberRule> ruleList = [];
List<List<int>> inputList = [];

var isReadingRules = true;
foreach (var input in Common.ReadInputLinesDefault())
{
    if (input == "")
    {
        if (!isReadingRules)
            break;
        isReadingRules = false;
        continue;
    }
    if (isReadingRules)
    {
        var numbers = input.Split("|").Select(int.Parse).ToList();
        ruleList.Add(new NumberRule(numbers[0], numbers[1]));
    }
    else
    {
        var numberList = input.Split(",").Select(int.Parse).ToList();
        inputList.Add(numberList);
    }
}

var correctNumbers = inputList.Where(nrList => ruleList.All(r => r.IsCorrect(nrList))).ToList();

var sum = correctNumbers.Sum(n => n[n.Count / 2]);
Console.WriteLine(sum);

// Part 2 
var incorrectNumbers = inputList.Where(nrList => ruleList.Any(r => !r.IsCorrect(nrList))).ToList();
foreach (var numberList in incorrectNumbers)
{
    while (ruleList.Any(r => !r.IsCorrect(numberList)))
    {
        var incorrectRules = ruleList.Where(r => !r.IsCorrect(numberList)).ToList();
        Console.WriteLine($"Number of incorrect rules: {incorrectRules.Count}");
        incorrectRules.ForEach(r =>
        {
            var lowerIndex = r.LowerIndex(numberList);

            numberList.RemoveAt(lowerIndex);
            numberList.Insert(lowerIndex, r.Higher);

            var higherIndex = r.HigherIndex(numberList);
            numberList.RemoveAt(higherIndex);
            numberList.Insert(higherIndex, r.Lower);
        });
    }
}

var sumPart2 = incorrectNumbers.Sum(n => n[n.Count / 2]);
Console.WriteLine(sumPart2);

record NumberRule(int Lower, int Higher)
{
    public bool IsCorrect(List<int> numbers)
    {
        var lowerIndex = LowerIndex(numbers);
        var higherIndex = HigherIndex(numbers);
        if (lowerIndex < 0 || higherIndex < 0) return true;
        return lowerIndex < higherIndex;
    }

    public int LowerIndex(List<int> numbers) => numbers.FindIndex(x => x == Lower);
    public int HigherIndex(List<int> numbers) => numbers.FindIndex(x => x == Higher);

}


