// List<Entity> entities = Common.ReadInputDefault().SelectMany((digitCh, index) =>
// {
//     long ID = 0;
//     var digit = int.Parse(digitCh.ToString());
//     var isFile = index % 2 == 0;

//     if (isFile)
//     {
//         ID = Utils.GetNextId();
//     }
//     List<Entity> entities = [];
//     while (entities.Count < digit)
//         entities.Add(isFile ? new File(ID) : new Empty());

//     return entities;
// }).ToList();

// while (entities.CountEmptyRight() != entities.CountEmpty())
// {
//     var nextEmpty = entities.FindIndex(x => x is Empty);
//     var nextFile = entities.FindLastIndex(x => x is File);
//     entities.SwapIndexes(nextEmpty, nextFile);
// }

// var sum = entities.TakeWhile(x => x is File).Select(static (f, i) => (f as File).ID * i).Sum();

// Console.WriteLine(sum);

// PART 2
Utils.ResetID();
var entities2 = Common.ReadInputDefault().SelectMany((digitCh, index) =>
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

System.IO.File.WriteAllText("before.txt", entities2.print());
var uniqueIds = entities2.GetUniqueIds().Order().Reverse().ToList();


foreach (var FileID in uniqueIds)
{
    Console.WriteLine($"[{FileID}] Handling");
    var files = entities2.GetFilesWithIndexById(FileID);
    var match = entities2.GetEmptyBlocks().ToList().Find((x) => x.Count >= files.Count);
    if (match is null)
    {
        Console.WriteLine($"[{FileID}] No Matches");
        continue;
    }
    Console.WriteLine($"[{FileID}] Found Matches");
    for (int i = 0; i < files.Count; i++)
    {
        entities2.SwapIndexes(match[i], files[i].index);
    }
}
System.IO.File.WriteAllText("after.txt", entities2.print());



var sum2 = entities2.Select(static (f, i) =>
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
        List<int> fileBlock = [];
        List<File> fileBlockFiles = [];
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            var entity = entities[i];
            if (entity is Empty)
            {
                if (fileBlock.Count > 0)
                {
                    yield return fileBlock;
                    fileBlock = [];
                    fileBlockFiles = [];
                }
            }

            if (entity is File file)
            {
                if (!fileBlockFiles.All(x => x.ID == file.ID))
                {
                    yield return fileBlock;
                    fileBlock = [];
                    fileBlockFiles = [];
                }

                fileBlock.Add(i);
                fileBlockFiles.Add(file);
            }
        }
    }

    public static IEnumerable<List<int>> GetEmptyBlocks(this List<Entity> entities)
    {
        var currentIndex = 0;
        while (true)
        {
            currentIndex = entities.FindIndex(currentIndex, x => x is Empty);
            if (currentIndex < 0)
                break;
            var size = entities.Skip(currentIndex).TakeWhile(x => x is Empty).Count();
            yield return Enumerable.Range(currentIndex, size).ToList();
            currentIndex += size;


        }
    }

    public static List<File> GetFiles(this List<Entity> entities) => entities.OfType<File>().ToList();
    public static List<(File file, int index)> GetFilesWithIndex(this List<Entity> entities)
    {
        return entities
            .WithIndex()
            .Where(x => x.Item1 is File)
            .Select(x => ((File)x.Item1, x.Item2))
            .ToList();
    }
    public static List<(File file, int index)> GetFilesWithIndexById(this List<Entity> entities, long ID)
    {
        return entities.GetFilesWithIndex().Where(x => x.file.ID == ID).ToList();
    }
    public static List<(Entity, int)> WithIndex(this List<Entity> entities) => entities.Select((v, i) => (entity: v, index: i)).ToList();
    public static List<File> GetFilesByID(this List<Entity> entities, long ID) => entities.GetFiles().Where(x => x.ID != ID).ToList();
    public static List<long> GetUniqueIds(this List<Entity> entities) => entities.GetFiles().Select(x => x.ID).Distinct().ToList();

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