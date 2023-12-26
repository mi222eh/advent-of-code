open System.Linq
open System

let IsZero n = n = (int64)0
let IsNotZero n = n <> (int64)0

let ToNumber (i:string) = int64(i)
let ToString (i:int64) = i.ToString()

let JoinWithTab (a:string) = fun b -> $"{a}\t{b}"

let lines = Common.ReadInputLines "input.txt" 
            |> Array.toList 
            |> List.map(fun n -> n.Split(' ') |> Array.toList |> List.map ToNumber)
            |> List.map (fun n ->
                    let mutable predictions: int64 list list = [n]
                    while predictions.Last().Any IsNotZero do
                        let last = predictions.Last()
                        let mutable next: int64 list = []
                        for n in 1..last.Length - 1 do
                            next <- next.Append(last[n] - last[n-1]) |> Seq.toList
                        predictions <- predictions.Append(next) |> Seq.toList
                    predictions
                )
            |> List.map( fun nrL -> 
                    let mutable newL: int64 list list = [];
                    let reversed = nrL |> List.rev
                    for i = 0 to reversed.Length - 1 do
                        let currentList = reversed[i]
                        let mutable nrToAdd = (int64)0;
                        if currentList.All IsZero then
                            nrToAdd <- 0
                        else
                            let previousList = newL[i - 1]
                            nrToAdd <- currentList.Last() + previousList.Last()
                        newL <- newL.Append(currentList.Append(nrToAdd) |> Seq.toList) |> Seq.toList
                    newL |> List.rev
                )
            |> List.map( fun nrL -> 
                    let mutable newL: int64 list list = [];
                    let reversed = nrL |> List.rev
                    for i = 0 to reversed.Length - 1 do
                        let currentList = reversed[i]
                        let mutable nrToAdd = (int64)0;
                        if currentList.All IsZero then
                            nrToAdd <- 0
                        else
                            let previousList = newL[i - 1]
                            nrToAdd <- currentList.First() - previousList.First()
                        newL <- newL.Append(currentList.Prepend(nrToAdd) |> Seq.toList) |> Seq.toList
                    newL |> List.rev
                )
    
let numberList = lines
                |> List.map(fun n -> n.First().Last())

let part2NumberList = lines
                    |> List.map(fun n -> n.First().First())


let result = numberList |> List.reduce(fun a -> fun b-> 
    let result = a+b
    result
)


let part2Result = part2NumberList |> List.sum

let debugText = lines 
                |> List.map(fun l -> 
                    let result = l.First().Last()
                    let mutable lines = ""
                    for nrs in l do 
                        let txt = nrs |> List.map ToString |> List.reduce JoinWithTab
                        lines <- $"{lines}\n{txt}"
                    $"{lines}\nResult for this is {result}"
                    )

Common.WriteLinesOutput debugText

printfn $"Result: {result}"
printfn $"Result 2: {part2Result}"
Console.ReadKey true |> ignore