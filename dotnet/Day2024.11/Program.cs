// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;

var stones = Common.ReadInputDefault().Split(" ").Select(UInt128.Parse).Select(x => new Stone(x, 1)).ToList();
for (int i = 0; i < 75; i++)
{
    Console.WriteLine($"[{i + 1}]");
    var newStones = new List<Stone>();
    foreach (var stone in stones)
    {
        var nextStones = stone.GetNext();
        foreach (var nextStone in nextStones)
        {
            var existing = newStones.Find(x => nextStone.Value == x.Value);
            if (existing is not null)
            {
                existing.AddAmount(nextStone.Amount);
            }
            else
            {
                newStones.Add(nextStone);
            }
        }
    }
    stones = newStones;
}

Console.WriteLine(stones.Select(x => x.Amount).Aggregate((c, v) => c + v));


class Stone(UInt128 value, UInt128 amount)
{
    public UInt128 Value = value;
    public UInt128 Amount = amount;
    public List<Stone> GetNext()
    {
        if (value == 0) return [new(1, Amount)];

        var valueStr = value.ToString();
        if (value.ToString().Length % 2 == 0)
            return [new(UInt128.Parse(valueStr[..(valueStr.Length / 2)]), Amount), new(UInt128.Parse(valueStr[(valueStr.Length / 2)..]), Amount)];

        return [new(value * 2024, Amount)];
    }

    public void AddAmount(UInt128 amount)
    {
        Amount += amount;
    }

}