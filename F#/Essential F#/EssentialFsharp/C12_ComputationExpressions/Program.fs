open ComputationExpression.OptionDemo

[<EntryPoint>]
let main argv =
    calculate 8 0 |> printfn "calculate 8 0 = %A"
    calculate 8 2 |> printfn "calculate 8 2 = %A"
    0
