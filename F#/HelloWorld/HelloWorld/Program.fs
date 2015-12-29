// Дополнительные сведения о F# см. на http://fsharp.org
// Дополнительную справку см. в проекте "Учебник по F#".

let SayHello() =
    printfn "Hello"

[<EntryPoint>]
let main argv = 
    SayHello()
    System.Console.ReadKey() |> ignore
    0 // возвращение целочисленного кода выхода
