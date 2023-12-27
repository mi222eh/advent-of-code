module Common

open System.IO
open System.Runtime.CompilerServices
open System

let public IsDebugMode =
#if DEBUG
    true
#else
    false
#endif

let BaseDirectory =
    match IsDebugMode with
    | false -> Environment.CurrentDirectory
    | true -> Path.Join(Environment.CurrentDirectory, "..", "..", "..")

let FromBaseDirectory path = Path.Join(BaseDirectory, path)

let public ReadInput (input: string) =
    File.ReadAllText(FromBaseDirectory input)

let public ReadInputLines (input: string) =
    File.ReadAllLines(FromBaseDirectory input)

let public WriteLinesOutput (input: string list) =
    File.WriteAllLines(FromBaseDirectory "output.txt", input)

let public Trim (input: string) = input.Trim()

let public FilterEmpty (input: string) =
    isNull (input) = false && input.Length > 0

let public IsSome<'T> (input: Option<'T>) =
    match input with
    | Some(value) -> true
    | None -> false

[<Extension>]
type StringExtensions =
    [<Extension>]
    static member Remove(x: string, toRemove: string) = x.Replace(toRemove, "")
