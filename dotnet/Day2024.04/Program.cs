
var WORD = "XMAS";
var WORD_LIST = WORD.ToCharArray().ToList();

var board = new Board();

Common.ReadInputLinesDefault().Select((v, i) => (v, i)).ToList().ForEach((args) => board.AddLine(args.v, args.i));

var sum = board.Iter().Select(args =>
{
    var count = Enum.GetValues<Direction>().Where(direction =>
    {
        var currentCoord = args.coord;
        var currentChar = args.ch;
        if (currentChar != WORD_LIST.First()) return false;
        return WORD_LIST.Skip(1).All(ch =>
        {
            try
            {
                currentCoord = board.GetRelativeCoord(currentCoord, direction);
                currentChar = board.GetChar(currentCoord);
            }
            catch
            {
                return false;
            }
            return ch == currentChar;
        });
    }).Count();
    return count;
}).Sum();

Console.WriteLine(sum);

// Part 2

List<char> letterPart2 = ['M', 'S'];
List<Direction> directionsPart2 = [global::Direction.UP_LEFT, global::Direction.UP_RIGHT, global::Direction.DOWN_LEFT, global::Direction.DOWN_RIGHT];

var sumPart2 = board.IterChar('A').Where(coord =>
{
    try
    {
        if (!directionsPart2.All(dir => letterPart2.Any(c => board.GetRelativeChar(coord, dir) == c)))
            return false;

        if (board.GetRelativeChar(coord, Direction.UP_LEFT) == 'M' && board.GetRelativeChar(coord, Direction.DOWN_RIGHT) != 'S')
            return false;
        if (board.GetRelativeChar(coord, Direction.UP_LEFT) == 'S' && board.GetRelativeChar(coord, Direction.DOWN_RIGHT) != 'M')
            return false;
        if (board.GetRelativeChar(coord, Direction.DOWN_LEFT) == 'M' && board.GetRelativeChar(coord, Direction.UP_RIGHT) != 'S')
            return false;
        if (board.GetRelativeChar(coord, Direction.DOWN_LEFT) == 'S' && board.GetRelativeChar(coord, Direction.UP_RIGHT) != 'M')
            return false;

        return true;
    }
    catch { return false; }
}).Count();

Console.WriteLine(sumPart2);

enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    UP_LEFT,
    UP_RIGHT,
    DOWN_LEFT,
    DOWN_RIGHT
}


record Coord(int x, int y);
class Board
{
    Dictionary<int, List<char>> letterDic = new();

    public void AddLetter(char letter, int y)
    {
        if (!letterDic.ContainsKey(y))
            letterDic[y] = new();

        var letterList = letterDic[y];
        letterList.Add(letter);
    }
    public char GetChar(Coord coord) => letterDic[coord.y][coord.x];
    public char GetRelativeChar(Coord coord, Direction direction) => GetChar(GetRelativeCoord(coord, direction));
    public Coord GetRelativeCoord(Coord coord, Direction direction) => direction switch
    {
        Direction.UP => new Coord(coord.x, coord.y + 1),
        Direction.DOWN => new Coord(coord.x, coord.y - 1),
        Direction.LEFT => new Coord(coord.x - 1, coord.y),
        Direction.RIGHT => new Coord(coord.x + 1, coord.y),
        Direction.DOWN_LEFT => new Coord(coord.x - 1, coord.y - 1),
        Direction.DOWN_RIGHT => new Coord(coord.x + 1, coord.y - 1),
        Direction.UP_LEFT => new Coord(coord.x - 1, coord.y + 1),
        Direction.UP_RIGHT => new Coord(coord.x + 1, coord.y + 1),
        _ => throw new Exception("Invalid Direction")
    };
    public bool IsLetter(char ch, Coord coord) => GetChar(coord) == ch;

    public void AddLine(string line, int y) =>
        line.ToCharArray()
            .ToList()
            .Select((ch, index) => (ch, index))
            .ToList()
            .ForEach((tuple) => AddLetter(tuple.ch, y));

    public IEnumerable<(char ch, Coord coord)> Iter()
    {
        foreach (var y in letterDic.Keys)
        {
            for (int x = 0; x < letterDic[y].Count; x++)
            {
                var ch = letterDic[y][x];
                yield return (ch, new Coord(x, y));
            }
        }
    }
    public IEnumerable<Coord> IterChar(char ch)
    {
        foreach (var args in Iter())
        {
            if (ch == args.ch)
                yield return args.coord;
        }
    }
}