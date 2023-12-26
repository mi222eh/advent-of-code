open Pipe


type Direction = 
    | North
    | South
    | East
    | West

let FromInput s = match s with 
                        | '|' -> Vertical
                        | '-' -> Horizontal
                        | 'L' -> NorthEast
                        | 'J' -> SouthWest
                        | 'F' -> SouthEast
                        | '.' -> Ground
                        | 'S' -> StartingPoint



type Pipe(p) = 
    member val Direction:PipeDirection = p with get, set
    member val IsStart = p = StartingPoint 

type PipeMap(map) = 
    member val map:Pipe array array = map

    member this.GetStartingPoint() = this.map |> Array.pick (fun list -> list |> Array.tryFind (fun pipe -> pipe.IsStart))
    member this.GetPipeCoords(pipe:Pipe) = 
        let mutable xCord = 0
        let mutable yCord = 0
        for y in 0..this.map.Length - 1 do
            let list = this.map[y]
            for x in 0..list.Length - 1 do
                let p = list[x]
                if p = pipe then
                    xCord <- x
                    yCord <- y
        (xCord, yCord)
                    
let ToCharArray (s:string) = s.ToCharArray()
let ToPipe(c:char) = new Pipe(FromInput c)
let ToPipeArray(c:char array) = c |> Array.map ToPipe
let IsStatingPoint(t:Pipe) = t.IsStart
let FindStartingPoint (c:Pipe array) = c |> Array.tryFindIndex IsStatingPoint


let PipeMap = new PipeMap(Common.ReadInputLines "input.txt" |> Array.map ToCharArray |> Array.map ToPipeArray)

let GetPipeFrom (pipe:Direction, direction:Direction) = 
    