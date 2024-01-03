open System.Text

type Status =
    | Broken
    | Working
    | Unknown

type Row(statuses: Status array, pattern: int array) =
    member val Statuses = statuses with get
    member val Pattern = pattern with get

let rows =
    Common.ReadInputLines "input.txt"
    |> Seq.map (fun line ->
        // split by space, first part is statuses, second part is pattern
        let parts = line.Split [| ' ' |]

        // ". = working, # = broken, ? = unknown"

        // statuses
        let statuses =
            parts.[0].ToCharArray()
            |> Array.map (fun c ->
                match c with
                | '.' -> Working
                | '#' -> Broken
                | '?' -> Unknown
                | _ -> failwith "invalid status")

        // pattern
        let pattern = parts.[1].ToCharArray() |> Array.map int

        // create row
        let row = Row(statuses, pattern)

        row)
    |> Seq.toArray

let printRow (row: Row) =
    let statuses = row.Statuses
    let pattern = row.Pattern

    let sb = new StringBuilder()

    for i = 0 to statuses.Length - 1 do
        let status = statuses.[i]

        let c =
            match status with
            | Broken -> "[B]"
            | Working -> "[W]"
            | Unknown -> "[?]"

        sb.Append(c) |> ignore

    sb.ToString()

let printRows (rows: Row array) =
    rows |> Array.map printRow |> Array.iter (printfn "%s")

printRows rows
Common.PressAnyKeyToContinue()
