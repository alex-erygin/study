// Object Expressions
open System

// интерфейс
type ILogger =
    abstract member Info : string -> unit
    abstract member Error : string -> unit

// класс с реализацией
type Logger() =
    interface ILogger with
        member this.Info(msg) = printfn "Info: %s" msg
        member this.Error(msg) = printfn "Error: %s" msg

// object expression        
let logger = {
    new ILogger with
        member this.Info(msg) = printfn "Info: %s" msg
        member this.Error(msg) = printfn "Error: %s" msg
}


type MyClass(logger: ILogger) =
    let mutable count = 0
    
    member this.DoSomething input =
        logger.Info $"Processing {input} at {DateTime.UtcNow.ToString()}"
        count <- count + 1
        ()
        
    member this.Count = count

let myClass = MyClass(logger)
[1..10] |> List.iter myClass.DoSomething
printfn "%i" myClass.Count


let doSomethingElse (logger:ILogger) input =
    logger.Info $"Processing {input} at {DateTime.UtcNow.ToString()}"
    ()

doSomethingElse logger "MyData"
