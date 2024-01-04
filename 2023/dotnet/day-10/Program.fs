open Pipe
open System


type Direction =
    | North
    | South
    | East
    | West

let FromInput s =
    match s with
    | '|' -> Vertical
    | '-' -> Horizontal
    | 'L' -> NorthEast
    | 'J' -> NorthWest
    | '7' -> SouthWest
    | 'F' -> SouthEast
    | '.' -> Ground
    | 'S' -> StartingPoint
    | _ -> failwith "Unknown char"



type Pipe(p, x, y) =
    member val Direction: PipeDirection = p with get, set
    member val IsStart = p = StartingPoint
    member val X: int = x
    member val Y: int = y

type PipeMap(map) =
    member val map: Pipe array array = map

    member this.GetStartingPoint() =
        this.map
        |> Array.pick (fun list -> list |> Array.tryFind (fun pipe -> pipe.IsStart))

    member this.GetPipeCoords(pipe: Pipe) =
        let mutable xCord = 0
        let mutable yCord = 0

        for y in 0 .. this.map.Length - 1 do
            let list = this.map[y]

            for x in 0 .. list.Length - 1 do
                let p = list[x]

                if p = pipe then
                    xCord <- x
                    yCord <- y

        (xCord, yCord)

    member this.GetPipe(x: int, y: int) =
        try
            Some(this.map[y][x])
        with :? System.IndexOutOfRangeException ->
            None

    member this.GetPipe(pipe: Pipe, dir: Direction) =
        let x, y = this.GetPipeCoords(pipe)

        match dir with
        | North -> this.GetPipe(x, y - 1)
        | South -> this.GetPipe(x, y + 1)
        | West -> this.GetPipe(x - 1, y)
        | East -> this.GetPipe(x + 1, y)

    member this.GetPipeIfConnected(pipe: Pipe, dir: Direction) =
        let otherPipeOption = this.GetPipe(pipe, dir)

        if otherPipeOption.IsSome then
            let otherPipe = otherPipeOption.Value

            if otherPipe.IsStart then
                Some(otherPipeOption.Value)
            else
                match dir with
                | North ->
                    if List.contains otherPipe.Direction [ SouthEast; SouthWest; Vertical ] then
                        Some(otherPipe)
                    else
                        None
                | South ->
                    if List.contains otherPipe.Direction [ NorthEast; NorthWest; Vertical ] then
                        Some(otherPipe)
                    else
                        None
                | East ->
                    if List.contains otherPipe.Direction [ NorthWest; SouthWest; Horizontal ] then
                        Some(otherPipe)
                    else
                        None
                | West ->
                    if List.contains otherPipe.Direction [ NorthEast; SouthEast; Horizontal ] then
                        Some(otherPipe)
                    else
                        None
        else
            None

    member this.GetConnectedPipes(pipe: Pipe) =
        let directions: Direction list =
            match pipe.Direction with
            | StartingPoint -> [ North; West; South; East ]
            | NorthEast -> [ North; East ]
            | NorthWest -> [ North; West ]
            | SouthEast -> [ South; East ]
            | SouthWest -> [ South; West ]
            | Horizontal -> [ West; East ]
            | Vertical -> [ North; South ]
            | Ground -> []

        directions
        |> List.map (fun d -> this.GetPipeIfConnected(pipe, d))
        |> List.filter Common.IsSome
        |> List.map (fun d -> d.Value)

    member this.GetSurroundingPipes(pipe: Pipe) =
        [ this.GetPipe(pipe, North)
          this.GetPipe(pipe, South)
          this.GetPipe(pipe, East)
          this.GetPipe(pipe, West) ]
        |> List.filter Common.IsSome
        |> List.map (fun d -> d.Value)

let ToCharArray (s: string) = s.ToCharArray()
let ToPipe (c: char, x, y) = new Pipe(FromInput c, x, y)

let ToPipeArray (c: char array, y) =
    c |> Array.mapi (fun x -> fun c -> ToPipe(c, x, y))


let PipeMap =
    new PipeMap(
        Common.ReadInputLines "input.txt"
        |> Array.map ToCharArray
        |> Array.mapi (fun y -> fun c -> ToPipeArray(c, y))
    )

let start = PipeMap.GetStartingPoint()
let mutable path = [ start ]

let IsDone () = path[0] <> path[path.Length - 1]

let GetNextPipe () =
    let lastPipe = path[path.Length - 1]
    let connectedPipes = PipeMap.GetConnectedPipes(lastPipe)
    let notIncluded = connectedPipes |> List.except path

    if notIncluded.Length = 0 then
        path <- List.append path [ start ]
    else
        path <- List.append path [ notIncluded[0] ]


let rec loop condition action =
    action ()

    if condition () then
        loop condition action
    else
        Console.WriteLine "Loop Stopped"

loop IsDone GetNextPipe



let nrOfSteps = path.Length
let furthest = Math.Floor((decimal) nrOfSteps / (decimal) 2)
Console.WriteLine $"There are {nrOfSteps} steps in the pipe"
Console.WriteLine $"Result: {furthest} steps"
Console.ReadKey true |> ignore

let GetEncapsulatedPipes () =
    let mutable result = []

    for y in 0 .. PipeMap.map.Length - 1 do
        let list = PipeMap.map[y]

        for x in 0 .. list.Length - 1 do
            let p = list[x]

            if p.Direction <> StartingPoint then
                let surroundingPipes = PipeMap.GetSurroundingPipes(p)
                let notIncluded = surroundingPipes |> List.except result

                result <- List.append result notIncluded

    result
