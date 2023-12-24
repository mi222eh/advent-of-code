open System.IO
open System.Linq
open System

type Node(value, l, r) = 
    member this.Value:string = value
    member this.L:string = l
    member this.R:string = r

    member this.EndsWith(value:string) = 
        this.Value.EndsWith(value)

type NodeList(nodes_str) = 
    member this.Nodes = nodes_str |> List.map(fun(a:string) -> 
                        let parts = a.Replace("(", "").Replace(")", "").Split('=')
                        let value = parts.[0]
                        let lr = parts.[1].Split(',')
                        let l = lr.[0]
                        let r = lr.[1]
                        Node(value, l, r)
                    )
    member this.FindManyByValueEndsWith(value:string) = this.Nodes |> List.filter(fun x -> x.Value.EndsWith(value))
    member this.FindByValue(value:string) = this.Nodes.First(fun x -> x.Value = value)

let dirs, nodes_str = File.ReadAllLines("../../../input.txt") 
                        |> Array.toList 
                        |> function 
                            | instructions :: _ :: nodes_str  -> (instructions, nodes_str)
                            | _ -> failwith("Invalid input")

let node_list = NodeList(nodes_str)


let mutable current_nodes = node_list.FindManyByValueEndsWith("A")
let mutable current_step = 0
let mutable loop = true

while loop do
    if (current_nodes.All(fun x -> x.Value.EndsWith('Z')) = false) then
        loop <- false
    else
        current_nodes <- match dirs[current_step % dirs.Length] with
                            | 'L' -> current_nodes |> List.map(fun x -> node_list.FindByValue(x.L))
                            | 'R' -> current_nodes |> List.map(fun x -> node_list.FindByValue(x.R))
                            | _ -> failwith("Unkonwn dir")
        current_step <- current_step + 1

printfn($"{current_step}")
Console.ReadKey(true)