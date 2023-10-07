type Customer = { 
    Id:string; 
    IsEligible:bool; 
    IsRegistered:bool
}

let calculateTotal customer spend =
    let discount = 
        if customer.IsEligible && spend >= 100.0M
        then (spend * 0.1M) else 0.0M
    spend - discount

let john = { Id = "John"; IsEligible = true; IsRegistered=true }
let mary = { Id = "Mary"; IsEligible = true; IsRegistered = true}
let richard = { Id = "Richard"; IsEligible = false; IsRegistered = true;}

let areEqual expected actual = 
    actual = expected

let sarah = { Id = "Sarah"; IsEligible = false; IsRegistered = false;}
let assertJohn = areEqual (calculateTotal john 100.M) 90.0M
let assertMAry = areEqual (calculateTotal mary 99.0M) 99.0M
let assertRichard = areEqual (calculateTotal richard 100.M) 100.0M
let assertSarah = areEqual (calculateTotal sarah 100.0M) 100.0M


