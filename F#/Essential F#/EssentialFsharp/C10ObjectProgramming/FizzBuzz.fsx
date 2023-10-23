// CLASSES

type FizzBuzz() =
    member this.Calculate(value) =
        [(3, "Fizz");(5, "Buzz")]
        |> List.map( fun (v,s) -> if value % v = 0 then s else "" )
        |> List.reduce (+)
        |> fun s -> if s = "" then string value else s

let fizzBuzz = FizzBuzz()


let fifteen = 15 |> fizzBuzz.Calculate

let doFizBuzz range =
    let fizzBuzz = FizzBuzz()
    range
    |> List.map(fizzBuzz.Calculate)


let output = doFizBuzz [1..15]

type FizzBuzz2(mapping) =
    member _.Calculate(value) =
        mapping
        |> List.map ( fun (v,s) -> if value % v = 0 then s else "")
        |> List.reduce (+)
        |> fun s -> if s = "" then string value else s

let doFizzBuzz2 mapping range =
    let fizzBuzz = FizzBuzz2(mapping)
    range
    |> List.map fizzBuzz.Calculate
    
let output2 = doFizzBuzz2 [(3, "Fizz");(5, "Buzz")] [1..15]

type FizzBuzz3(mapping)=
    let calculate n =
        mapping
        |> List.map( fun (v,s) -> if n % v = 0 then s else "")
        |> List.reduce (+)
        |> fun s -> if s = "" then string n else s
        
    member _.Calculate(value) = calculate value

let doFizzBuzz3 mapping range =
    let fizzBuzz = FizzBuzz3(mapping)
    range
    |> List.map fizzBuzz.Calculate
    
let output3 = doFizzBuzz2 [(3, "Fizz");(5, "Buzz")] [1..15]

// INTERFACES
type IFizzBuzz =
    abstract member Calculate : int -> string
    
type FizzBuzz4(mapping) =
    let calculate n =
        mapping
        |> List.map( fun (v,s) -> if n % v = 0 then s else "")
        |> List.reduce (+)
        |> fun s -> if s = "" then string n else s
        
    interface IFizzBuzz with
        member _.Calculate(value) = calculate value
        
let doFizzBuzz4 =
    let fizzBuzz = FizzBuzz4([(2,"Fizz");(5,"Buzz")]) :> IFizzBuzz // Upcast
    [1..15]
    |> List.map (fizzBuzz.Calculate)
