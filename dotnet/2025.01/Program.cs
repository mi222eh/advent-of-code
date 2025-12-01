


using System.IO.Compression;

var inputLines = Common.ReadInputLinesDefault();

var combLock = new Lock();
var combination = inputLines.Select(i => Entry.FromInput(i));

var combo = new List<int>();
foreach (var comb in combination)
{
    combo.Add(combLock.InputEntry(comb));
}

Console.WriteLine(combo.Where(x => x == 0).Count());
Console.WriteLine(combLock.NrOfTimes0Crossed);

class Lock
{
    int Nr = 50;
    public int NrOfTimes0Crossed { get; set; } = 0;

    public int InputEntry(Entry entry)
    {
        var steps = entry.Steps;
        var direction = entry.Direction;

        while (steps > 0)
        {
            InputStep(direction);
            if (Nr == 0)
            {
                NrOfTimes0Crossed++;
            }
            steps--;
        }
        return Nr;
    }

    public void InputStep(Direction direction)
    {
        if (direction == Direction.Left)
        {
            if (Nr == 0)
            {
                Nr = 99;
            }
            else
            {
                Nr = Nr - 1;
            }
        }
        else
        {
            if (Nr == 99)
            {
                Nr = 0;
            }
            else
            {
                Nr = Nr + 1;
            }
        }
    }
}

record Entry(Direction Direction, int Steps)
{
    public static Entry FromInput(string input) => new Entry(input[0].ToDirection(), int.Parse(input.Substring(1)));
};

enum Direction
{
    Left,
    Right
}

static class Extensions
{
    extension(char source)
    {
        public Direction ToDirection() => source switch
        {
            'L' => Direction.Left,
            'R' => Direction.Right,
            _ => throw new Exception()
        };
    }
}
