type RegisteredCustomer = {
    Id: string
    IsEligible: bool
}

type UnregisteredCustomer = {
    Id: string
}

type Customer =
    | Registered of Id:string * IsEligible:bool
    | Guest of Id: string

let sarah = Guest ( Id = "Sarah" )
let john = Registered ( Id = "John", IsEligible = true )
let mary = Registered ( Id = "Mary", IsEligible = true )
let richard = Registered ( Id = "Richard", IsEligible = false )

let (|IsEligible|_|) customer =
    match customer with
    | Registered (IsEligible = true) -> Some()
    | _ -> None

let calculateTotal customer spend =
    let discount =
        match customer with
        | IsEligible when spend >= 100.0M -> spend * 0.1M
        | _ -> 0.0M
    spend - discount

let areEqual expected actual = 
    actual = expected
 
let assertJohn = areEqual (calculateTotal john 100.M) 90.0M
let assertMAry = areEqual (calculateTotal mary 99.0M) 99.0M
let assertRichard = areEqual (calculateTotal richard 100.M) 100.0M
let assertSarah = areEqual (calculateTotal sarah 100.0M) 100.0M