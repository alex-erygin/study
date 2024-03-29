open System

let tryDivide (x:decimal) (y:decimal) =
    try
        Ok(x/y)
    with
    | :? DivideByZeroException as ex -> Error ex
    
let badDivide = tryDivide 1M 0M

let goodDivide = tryDivide 1M 1M