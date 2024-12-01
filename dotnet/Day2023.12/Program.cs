






var statusRows = Common.ReadInputLines("input.txt").Select(l =>
{
    var splitted = l.Split(' ');
    var statuses = splitted.First().Select(c => (Status)c).ToList();
    var brokenPattern = splitted.Last().Split(',').Select(c => int.Parse(c.ToString())).ToList();
    return new StatusRow(statuses, brokenPattern);
}).ToList();


var combinationsTried = 0;
var combinationsFound = 0;
var rowsSolved = 0;
var totalRows = statusRows.Count();
var sum = 0;




void PrintStatus()
{
    Console.SetCursorPosition(0, 0);
    var progress = Math.Round((double)rowsSolved / totalRows * 100, 2);
    var toPrint = "";
    toPrint += $"Combinations tried: {combinationsTried}\n";
    toPrint += $"Combinations found: {combinationsFound}\n";
    toPrint += $"Rows solved: {rowsSolved}\n";
    toPrint += $"Total rows: {totalRows}\n";
    toPrint += $"Percentage: {progress}%\n";
    toPrint += $"Sum: {sum}\n";
    Console.Write(toPrint);
    // Console.WriteLine($"Combinations tried: {combinationsTried}");
    // Console.WriteLine($"Combinations found: {combinationsFound}");
    // Console.WriteLine($"Rows solved: {rowsSolved}");
    // Console.WriteLine($"Total rows: {totalRows}");
    // Console.WriteLine($"Percentage: {Math.Round((double)rowsSolved / totalRows * 100, 2)}%");
    // Console.WriteLine($"Sum: {sum}");
}






async Task<List<string>> GetNrOfCombinations(StatusRow row, List<string>? combinations = default)
{
    combinations ??= new List<string>();
    combinationsTried++;
    var brokenPattern = row.BrokenPattern;
    var brokenGroups = row.FindBrokenGroups();
    var brokenGroupsStartIndexes = row.FindBrokenGroupsStartIndexes();

    if (brokenGroups.Count() == brokenPattern.Count())
    {
        if (brokenGroups.SequenceEqual(brokenPattern))
        {
            combinationsFound++;
            combinations.Add(row.ToString());
            return combinations;
        }
    }
    if (brokenGroups.Count() < brokenPattern.Count())
    {
        var freeSpots = row.ListFreeSpots();
        foreach (var spot in freeSpots)
        {
            var clonedRow = row.CloneRow();
            clonedRow.Statuses[spot] = Status.Broken;
            var nextCombinations = await GetNrOfCombinations(clonedRow);
            combinations.AddRange(nextCombinations);
        }
    }
    else if (brokenGroups.Count() > brokenPattern.Count())
    {
        return combinations;
    }
    else
    {
        for (var i = 0; i < brokenGroups.Count(); i++)
        {
            var currentSize = brokenGroups[i];
            var actualSize = brokenPattern[i];
            var startsAt = brokenGroupsStartIndexes[i];
            var nextUnknown = row.GetNextUnknown(startsAt);
            var previousUnknown = row.GetPreviousUnknown(startsAt);

            if (currentSize == actualSize) continue;
            else if (currentSize < actualSize)
            {
                if (nextUnknown != -1)
                {
                    var nextRow = row.CloneRow();
                    nextRow.Statuses[nextUnknown] = Status.Broken;
                    var nextCombinations = await GetNrOfCombinations(nextRow);
                    combinations.AddRange(nextCombinations);
                }
                if (previousUnknown != -1)
                {
                    var previousRow = row.CloneRow();
                    previousRow.Statuses[previousUnknown] = Status.Broken;
                    var previousCombinations = await GetNrOfCombinations(previousRow);
                    combinations.AddRange(previousCombinations);
                }
            }
            else if (currentSize > actualSize)
            {
                return combinations;
            }
        }
    }
    return combinations;
}


var isDone = false;
var printTask = Task.Run(async () =>
{
    Console.Clear();
    while (!isDone)
    {
        PrintStatus();
        await Task.Delay(200);
    }
});

// var test = await GetNrOfCombinations(statusRows[0]);

// Console.WriteLine(test.Count);
// Console.WriteLine(test.Distinct().Count());

statusRows.AsParallel().Select(async row =>
{
    var combinations = await GetNrOfCombinations(row);
    var distinctCombinations = combinations.Distinct();
    var nrOfCombinations = distinctCombinations.Count();
    var sumOfCombinations = distinctCombinations.Sum(c => c.Count());
    Interlocked.Add(ref sum, nrOfCombinations);
    Interlocked.Increment(ref rowsSolved);
}).ForAll(t => t.Wait());

isDone = true;
await printTask;
Console.WriteLine($"Sum: {sum}");

public enum Status
{
    Operational = '.',
    Broken = '#',
    Unknown = '?'
}

public record StatusRow(List<Status> Statuses, List<int> BrokenPattern)
{
    public override string ToString() => new string(Statuses.Select(s => (char)s).ToArray());

    public List<int> FindBrokenGroups()
    {
        var brokenGroups = new List<int>();
        var currentBrokenGroup = 0;
        foreach (var status in Statuses)
        {
            if (status == Status.Broken)
            {
                currentBrokenGroup++;
            }
            else
            {
                if (currentBrokenGroup > 0)
                {
                    brokenGroups.Add(currentBrokenGroup);
                    currentBrokenGroup = 0;
                }
            }
        }
        if (currentBrokenGroup > 0)
        {
            brokenGroups.Add(currentBrokenGroup);
        }
        return brokenGroups;
    }

    public List<int> FindBrokenGroupsStartIndexes()
    {
        var brokenGroupsStartIndexes = new List<int>();
        var currentBrokenGroupStartIndex = 0;

        for (var i = 0; i < Statuses.Count; i++)
        {
            if (Statuses[i] == Status.Broken)
            {
                currentBrokenGroupStartIndex++;
            }
            else
            {
                if (currentBrokenGroupStartIndex > 0)
                {
                    brokenGroupsStartIndexes.Add(i - currentBrokenGroupStartIndex);
                    currentBrokenGroupStartIndex = 0;
                }
            }
        }

        if (currentBrokenGroupStartIndex > 0)
        {
            brokenGroupsStartIndexes.Add(Statuses.Count - currentBrokenGroupStartIndex - 1);
        }

        return brokenGroupsStartIndexes;
    }

    public List<int> FindUnknownGroups()
    {
        var unknownGroups = new List<int>();
        var currentUnknownGroup = 0;
        foreach (var status in Statuses)
        {
            if (status == Status.Unknown)
            {
                currentUnknownGroup++;
            }
            else
            {
                if (currentUnknownGroup > 0)
                {
                    unknownGroups.Add(currentUnknownGroup);
                    currentUnknownGroup = 0;
                }
            }
        }
        if (currentUnknownGroup > 0)
        {
            unknownGroups.Add(currentUnknownGroup);
        }
        return unknownGroups;
    }

    public (int, int) FindUnknownGroupAround(int index)
    {
        var start = index;
        var end = index;

        while (start > 0 && Statuses[start] != Status.Unknown)
        {
            start--;
        }

        while (end < Statuses.Count && Statuses[end] != Status.Unknown)
        {
            end++;
        }

        return (start, end);
    }

    public bool IsNotSurroundedByBroken(int index)
    {
        var nrOfStatuses = Statuses.Count();

        if (Statuses[index] == Status.Broken) return false;
        if (index > 0 && Statuses[index - 1] == Status.Broken) return false;
        if (index < nrOfStatuses - 1 && Statuses[index + 1] == Status.Broken) return false;

        return true;
    }

    public List<int> ListFreeSpots()
    {
        var freeSpots = new List<int>();
        for (var i = 0; i < Statuses.Count; i++)
        {
            if (IsNotSurroundedByBroken(i))
            {
                freeSpots.Add(i);
            }
        }
        return freeSpots;
    }

    public int GetNextUnknown(int index)
    {
        for (var i = index + 1; i < Statuses.Count; i++)
        {
            if (Statuses[i] == Status.Unknown)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetPreviousUnknown(int index)
    {
        for (var i = index - 1; i >= 0; i--)
        {
            if (Statuses[i] == Status.Unknown)
            {
                return i;
            }
        }
        return -1;
    }

    public StatusRow CloneRow()
    {
        var clonedStatuses = new List<Status>();
        Statuses.ForEach(clonedStatuses.Add);
        return new StatusRow(clonedStatuses, BrokenPattern);
    }
}

