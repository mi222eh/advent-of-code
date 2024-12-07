// See https://aka.ms/new-console-template for more information

using System.Collections.ObjectModel;

Map map = new();

var inputLines = Common.ReadInputLinesDefault().ToList().AsReadOnly();

map.Init(inputLines);

map.visitedCoords.Add(map.guard.coord);

while (!map.IsCoordOutside(map.guard.coord))
{
    try
    {
        var next = map.guard.GetInFront();
        if (next is Obstacle)
        {
            map.guard.turn();
            continue;
        }
        if (next is Empty)
        {
            map.guard.Step();
            if (map.IsCoordOutside(map.guard.coord))
            {
                break;
            }
            map.visitedCoords.Add(map.guard.coord);
        }
    }
    catch
    {
        break;
    }
}

var distinct = map.visitedCoords.ToList().Distinct();
Console.WriteLine(distinct.Count());

// PART 2
Map placeholder = new();
placeholder.Init(inputLines);

var emptyList = placeholder.IterEmpty().ToList();
var maps = emptyList.Select(emptyCoord =>
{
    var map = new Map();
    map.Init(inputLines);
    map.ReplaceEntity(emptyCoord, new Obstacle());
    return map;
});

var results = maps.AsParallel().Select((map, i) =>
{
    while (!map.IsCoordOutside(map.guard.coord.GetRelativeCoord(map.guard.facing)))
    {
        if (map.CountOfSameWanderedSpots > 1_000)
        {
            Console.WriteLine($"[{i}] Valid obstacle");
            return true;
        }

        var next = map.guard.GetInFront();
        if (next is Obstacle)
        {
            map.guard.turn();
            continue;
        }
        if (next is Empty)
        {
            map.guard.Step();
            if (map.IsCoordOutside(map.guard.coord))
            {
                Console.WriteLine($"[{i}] Not valid");
                break;
            }
            map.visitedCoords.Add(map.guard.coord);
        }
    }
    return false;
});

Console.WriteLine(results.Count(x => x == true));
Common.PressAnyKeyToContinue();




enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

record MapEntity();
record Obstacle() : MapEntity;
record Empty() : MapEntity;
class Guard
{
    public Coord coord;
    public Direction facing;
    public Map map;

    public MapEntity GetInFront() => map.GetEntity(coord.GetRelativeCoord(facing));
    public void Step()
    {
        coord = coord.GetRelativeCoord(facing);
    }
    public void turn()
    {
        if (facing == Direction.UP)
            facing = Direction.RIGHT;
        else if (facing == Direction.RIGHT)
            facing = Direction.DOWN;
        else if (facing == Direction.DOWN)
            facing = Direction.LEFT;
        else
            facing = Direction.UP;
    }
};

class Map
{
    public Guard guard = new();
    public List<Coord> visitedCoords = new(100_000);
    public List<List<MapEntity>> entities = new(200);

    public MapEntity GetEntity(Coord coord) => entities[coord.Y][coord.X];

    public int CountOfSameWanderedSpots => visitedCoords.Count - visitedCoords.Distinct().Count();

    public IEnumerable<Coord> IterEmpty()
    {
        for (int Y = 0; Y < entities.Count; Y++)
        {
            var entityList = entities[Y];
            for (int X = 0; X < entityList.Count; X++)
            {
                Coord coord = new(X, Y);
                var entity = entityList[X];
                if (entity is Empty && coord != guard.coord)
                    yield return coord;
            }
        }
    }

    public void ReplaceEntity(Coord coord, MapEntity entity)
    {
        entities[coord.Y][coord.X] = entity;
    }
    public void Init(ReadOnlyCollection<string> inputLines)
    {
        guard = new();
        visitedCoords = new();
        entities = new();
        for (int Y = 0; Y < inputLines.Count(); Y++)
        {
            List<MapEntity> initEntities = new(200);
            entities.Add(initEntities);
            var line = inputLines[Y];
            line.ToCharArray().Select((v, i) => (v, i)).ToList().ForEach(args =>
            {
                switch (args.v)
                {
                    case '^':
                        guard = new()
                        {
                            coord = new Coord(args.i, Y),
                            facing = Direction.UP,
                            map = this
                        };
                        initEntities.Add(new Empty());
                        break;
                    case '#':
                        initEntities.Add(new Obstacle());
                        break;
                    case '.':
                        initEntities.Add(new Empty());
                        break;
                    default:
                        throw new Exception("");
                };
            });
        }
    }
    public bool IsCoordOutside(Coord coord) =>
        coord.Y < 0 ||
        coord.X < 0 ||
        coord.Y >= entities.Count ||
        coord.X >= entities[coord.Y].Count;

}


record Coord(int X, int Y)
{
    public string AsString() => $"{X}:{Y}";
    public Coord GetRelativeCoord(Direction direction) => direction switch
    {
        Direction.UP => new Coord(X, Y - 1),
        Direction.LEFT => new Coord(X - 1, Y),
        Direction.RIGHT => new Coord(X + 1, Y),
        Direction.DOWN => new Coord(X, Y + 1),
        _ => throw new Exception("Invalid")
    };
};
