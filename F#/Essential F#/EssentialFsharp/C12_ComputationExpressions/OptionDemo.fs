namespace ComputationExpression

[<AutoOpen>]
module Option =
    
    type OptionBuilder() =
        // Supports let!
        member _.Bind(x,f) = Option.bind f x
        //Supports return
        member _.Return(x) = Some x
        // Support return!
        member _.ReturnFrom(x) = x
        
    // Computation Expression for Option
    // Usage will be option {...}
    let option = OptionBuilder()
    
    

module OptionDemo =
    let multiply x y =
        x * y
    
    let divide x y =
        if y = 0 then None
        else Some (x / y)
        
    // let calculate x y =
    //     divide x y
    //     |> Option.map(fun v -> multiply v x)
    //     |> Option.bind(fun t -> divide t y)
     
    let calculate x y =
        option {
            let! v = divide x y
            let t = multiply v x
            let! r = divide t y
            return r
        }
 
