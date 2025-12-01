open Common
open System.Collections.Generic
open System.Linq
open System


type Node(value: string, l: string, r: string) =
    member val Value = value.Trim()
    member val Left = l.Trim()
    member val Right = r.Trim()

    static member FromInput(input: string) =
        input.Remove("(").Remove(")").Split("=")
        |> Array.map Trim
        |> Array.toList
        |> function
            | value :: lr :: _ -> value, lr
            | _ -> failwith "Invalid input"
        |> fun (value, lr) -> (value, lr.Split(","))
        |> fun (value, lr) -> new Node(value, lr.[0], lr.[1])



let dirs, nodes_str =
    ReadInputLines "input.txt"
    |> Array.toList
    |> function
        | dirs :: nodes -> dirs, nodes
        // code to handle one index and the rest
        | _ -> failwith "invalid input"

let nodes = nodes_str |> List.filter FilterEmpty |> List.map Node.FromInput

let nodeDict = nodes |> List.toSeq |> Seq.map (fun node -> node.Value, node) |> dict

let GetLeftNode (n: Node) = nodeDict.[n.Left]
let GetRightNode (n: Node) = nodeDict[n.Right]
let mutable current_nodes = nodes |> List.filter (fun n -> n.Value.EndsWith('A'))
let mutable loops = (uint32) 0
let mutable step_nr = 0

let max_step = dirs.Length

let GetTotalSteps () =
    (loops * (uint32) max_step) + (uint32) step_nr

while current_nodes.Where(fun n -> n.Value.EndsWith('Z')).Count()
      <> current_nodes.Count() do

    if step_nr = dirs.Length then
        step_nr <- 0
        loops <- loops + (uint32) 1

    let current_step = dirs[step_nr]

    match current_step with
    | 'R' -> current_nodes <- current_nodes |> List.map GetRightNode
    | 'L' -> current_nodes <- current_nodes |> List.map GetLeftNode
    | _ -> failwith "Unknown direction"

    printfn $"Current steps: {GetTotalSteps()}"
    step_nr <- step_nr + 1


printfn $"Result: {GetTotalSteps()}"
Console.ReadKey true |> ignore
