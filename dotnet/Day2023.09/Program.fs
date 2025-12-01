open System

let isZero n = n = 0L
let isNotZero n = n <> 0L

let toNumber (i: string) = int64 i
let toString (i: int64) = i.ToString()

let joinWithTab (a: string) (b: string) = sprintf "%s\t%s" a b

let buildPredictions (start: list<int64>) : list<list<int64>> =
    let rec loop acc =
        let last = List.last acc

        if List.exists (fun x -> x <> 0L) last then
            let next = [ for i in 1 .. (List.length last - 1) -> last.[i] - last.[i - 1] ]
            loop (acc @ [ next ])
        else
            acc

    loop [ start ]

let processNrL_append nrL =
    let reversed = List.rev nrL

    let rec loop i acc =
        if i >= List.length reversed then
            List.rev acc
        else
            let currentList = reversed.[i]

            let nrToAdd =
                if List.forall isZero currentList then
                    0L
                else
                    let previousList = List.last acc
                    List.last currentList + List.last previousList

            let newList = currentList @ [ nrToAdd ]
            loop (i + 1) (acc @ [ newList ])

    loop 0 []

let processNrL_prepend nrL =
    let reversed = List.rev nrL

    let rec loop i acc =
        if i >= List.length reversed then
            List.rev acc
        else
            let currentList = reversed.[i]

            let nrToAdd =
                if List.forall isZero currentList then
                    0L
                else
                    let previousList = List.last acc
                    List.head currentList - List.head previousList

            let newList = nrToAdd :: currentList
            loop (i + 1) (acc @ [ newList ])

    loop 0 []

let lines =
    Common.ReadInputLines "input.txt"
    |> Array.toList
    |> List.map (fun n -> n.Split(' ') |> Array.toList |> List.map toNumber)
    |> List.map buildPredictions
    |> List.map processNrL_append
    |> List.map processNrL_prepend

let numberList = lines |> List.map (fun n -> List.last (List.head n))
let part2NumberList = lines |> List.map (fun n -> List.head (List.head n))

let result = numberList |> List.reduce (fun a b -> a + b)
let part2Result = part2NumberList |> List.sum

let debugText =
    lines
    |> List.map (fun l ->
        let result = List.last (List.head l)

        let linesTxt =
            l
            |> List.map (fun nrs -> nrs |> List.map toString |> List.reduce joinWithTab)
            |> String.concat "\n"

        sprintf "%s\nResult for this is %d" linesTxt result)

Common.WriteLinesOutput debugText

printfn "Result: %d" result
printfn "Result 2: %d" part2Result
Console.ReadKey true |> ignore
