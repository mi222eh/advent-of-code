List<Entity> entities = Common.ReadInputDefault().SelectMany((digitCh, index) =>
{
    long ID = 0;
    var digit = int.Parse(digitCh.ToString());
    var isFile = index % 2 == 0;

    if (isFile)
    {
        ID = Utils.GetNextId();
    }
    List<Entity> entities = [];
    while (entities.Count < digit)
        entities.Add(isFile ? new File(ID) : new Empty());

    return entities;
}).ToList();

while (entities.CountEmptyRight() != entities.CountEmpty())
{
    var nextEmpty = entities.FindIndex(x => x is Empty);
    var nextFile = entities.FindLastIndex(x => x is File);
    entities.SwapIndexes(nextEmpty, nextFile);
}

var sum = entities.TakeWhile(x => x is File).Select(static (f, i) => (f as File).ID * i).Sum();

Console.WriteLine(sum);

// PART 2
Utils.ResetID();
entities = Common.ReadInputDefault().SelectMany((digitCh, index) =>
{
    long ID = 0;
    var digit = int.Parse(digitCh.ToString());
    var isFile = index % 2 == 0;

    if (isFile)
    {
        ID = Utils.GetNextId();
    }
    List<Entity> entities = [];
    while (entities.Count < digit)
        entities.Add(isFile ? new File(ID) : new Empty());

    return entities;
}).ToList();


System.IO.File.WriteAllText("before.txt", entities.print());
foreach (var fileBlock in entities.IterFiles())
{
    var matches = entities.IterEmptyBlocks().TakeWhile(x => x.Last() < fileBlock.Last()).Where((x) => x.Count >= fileBlock.Count).ToList();
    if (matches.Count == 0) continue;
    var matched = matches.First();
    for (int i = 0; i < fileBlock.Count; i++)
    {
        entities.SwapIndexes(matched[i], fileBlock[i]);
    }
}
System.IO.File.WriteAllText("after.txt", entities.print());



var sum2 = entities.Select(static (f, i) =>
{
    if (f is File file)
    {
        return i * file.ID;
    }
    return 0;
}).Sum();

Console.WriteLine(sum2);






record Entity;

record File(long ID) : Entity;
record Empty : Entity;


static class Utils
{
    private static int _nextId = 0;
    public static int GetNextId()
    {
        var currentID = _nextId;
        _nextId++;
        return currentID;
    }

    public static void ResetID()
    {
        _nextId = 0;
    }

    public static void SwapIndexes(this List<Entity> entities, int index1, int index2)
    {
        Entity temp = entities[index1];
        Entity temp2 = entities[index2];

        entities[index1] = temp2;
        entities[index2] = temp;
    }
    public static int CountEmptyRight(this List<Entity> entities)
    {
        var listTmp = new List<Entity>(entities);
        listTmp.Reverse();
        return listTmp.TakeWhile(x => x is Empty).Count();
    }

    public static int CountEmpty(this List<Entity> entities) => entities.Count(x => x is Empty);

    public static IEnumerable<List<int>> IterFiles(this List<Entity> entities)
    {
        List<int> fileBlock = new();
        List<File> fileBlockFiles = new();
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            var entity = entities[i];
            if (entity is Empty)
            {
                if (fileBlock.Count > 0)
                {
                    yield return fileBlock;
                    fileBlock = new();
                    fileBlockFiles = new();
                }
            }

            if (entity is File file)
            {
                if (!fileBlockFiles.All(x => x.ID == file.ID))
                {
                    yield return fileBlock;
                    fileBlock = new();
                    fileBlockFiles = new();
                }

                fileBlock.Add(i);
                fileBlockFiles.Add(file);
            }
        }
    }

    public static IEnumerable<List<int>> IterEmptyBlocks(this List<Entity> entities)
    {
        List<int> emptyBlock = new();
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity is File)
            {
                if (emptyBlock.Count > 0)
                {
                    yield return emptyBlock;
                    emptyBlock = new();
                }
            }
            if (entity is Empty empty)
            {
                emptyBlock.Add(i);
            }
        }

    }

    public static string print(this List<Entity> entities)
    {
        string toPrint = "";

        foreach (var entity in entities)
        {
            if (entity is File file)
            {
                toPrint += file.ID;
            }
            else
            {
                toPrint += "#";
            }
        }

        return toPrint;
    }
}