open System

type Customer = {
    Id: int
    IsVip: bool
    Credit: decimal
}

let getPurchases customer =
    try
        // imagine this function is fetching data from a Database
        let purchases =
            if customer.Id  % 2 = 0 then (customer, 120M)
            else (customer, 80M)
        Ok purchases
    with
    | ex -> Error ex
    
let tryPromoteToVip purchases =
    let customer, amount = purchases
    if amount > 100M then { customer with IsVip = true }
    else customer
    
let increaseCreditIfVip customer =
    try
        // imagine this function is fetching data from a Database
        let increase =
            if customer.IsVip then 100.0M else 50.0M
        Ok { customer with Credit = customer.Credit + increase }
    with
    | ex -> Error ex
    
 
 let upgradeCustomer customer =
     customer
     |> getPurchases
     |> function
        | Ok x->Ok(tryPromoteToVip x)
        | Error ex -> Error ex
     |> function
        | Ok x -> increaseCreditIfVip x
        | Error ex -> Error ex

let customerVIP = { Id = 1; IsVip = true; Credit = 100.0M }
let customerSTD = { Id = 2; IsVip = false; Credit = 100.0M }

let assertVIP = upgradeCustomer customerVIP = Ok { Id = 1; IsVip = true; Credit = 100.0M }
let assertSTDtoVIP = upgradeCustomer customerSTD = Ok { Id = 2; IsVip = true; Credit = 200.0M }
let assertSTD = upgradeCustomer { customerSTD with Id = 3; Credit = 50M } = Ok { Id = 3; IsVip = false; Credit = 100.0M }
