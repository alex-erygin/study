open System.IO

type Customer = {
    CustomerId : string
    Email: string
    IsEligible: string
    IsRegistered: string
    DateRegistered: string
    Discount: string
}


let parseLine (line: string): Customer option =
    match line.Split('|') with
    | [|customerId; email; eligible; registered; dateRegistered; discount|] ->
        Some {
            CustomerId = customerId
            Email = email
            IsEligible = eligible
            IsRegistered = registered
            DateRegistered = dateRegistered
            Discount = discount
        }
    | _ -> None


let rec parse (data:string seq) =
    data
    |> Seq.skip 1
    |> Seq.map parseLine
    |> Seq.choose id


let output data =
    data
    |> Seq.iter (fun x -> printfn $"%A{x}")


type DataReader = string -> Result<string seq, exn>


let readFile : DataReader =
    fun path ->
        try
            File.ReadLines(path)
            |> Ok
        with
        | ex -> Error ex


let import (dataReader: DataReader) path =
    match path |> dataReader with
    | Ok data -> data |> parse |> output
    | Error ex -> printfn $"Error: %A{ex.Message}"



let fakeDataReader : DataReader =
    fun _ ->
        seq {
            "CustomerId|Email|Eligible|Registered|DateRegistered|Discount"
            "John|john@test.com|1|1|2015-01-23|0.1"
            "Mary|mary@test.com|1|1|2018-12-12|0.1"
            "Richard|richard@nottest.com|0|1|2016-03-23|0.0"
            "Sarah||0|0||"
        }
        |> Ok
 


[<EntryPoint>]
let main argv =
    "C:\projects\study\F#\Essential F#\EssentialFsharp\C6\c6App\customers.csv"
    |> import readFile
    0
