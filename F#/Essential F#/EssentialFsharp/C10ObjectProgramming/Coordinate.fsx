type FizzBuzz() =
    member _.Calculate(value) =
        [(3, "Fizz");(5, "Buzz")]
        |> List.map (fun (v,s) -> if value % v = 0 then s else "")
        |> List.reduce (+)
        |> fun s -> if s = "" then string value else s