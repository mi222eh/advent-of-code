// See https://aka.ms/new-console-template for more information

using System.Numerics;

var input = Common.ReadInput("input.txt")!;

List<int> leftList = [];
List<int> rightList = [];

foreach (var row in input.Split("\r\n"))
{
    var rowItems = row.Split("   ").ToList();
    var left = int.Parse(rowItems.First());
    var right = int.Parse(rowItems.Last());
    
    leftList.Add(left);
    rightList.Add(right);
}

leftList.Sort();
rightList.Sort();

var total = 0;

for (int index = 0; index < leftList.Count; index++)
{
    var leftNr = leftList[index];
    var rightNr = rightList[index];
    total += int.Abs(leftNr - rightNr);
}


Console.WriteLine(total);
Common.PressAnyKeyToContinue();

var scores = leftList.Select(nrL => nrL * rightList.Count(nrR => nrL == nrR));
var totalPart2 = scores.Sum();


Console.WriteLine(totalPart2);
Common.PressAnyKeyToContinue();
