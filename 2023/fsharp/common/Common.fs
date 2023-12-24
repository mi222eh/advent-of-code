module Common

open System.IO
open System.Runtime.CompilerServices

let public ReadInput (input: string) = File.ReadAllText($"{input}")    
let public ReadInputLines (input: string) = File.ReadAllLines($"{input}")

let public Trim (input:string) = input.Trim()
let public FilterEmpty (input:string) = isNull(input) = false && input.Length > 0

[<Extension>]
type StringExtensions =
    [<Extension>]
    static member Remove(x: string, toRemove: string) = x.Replace(toRemove, "")