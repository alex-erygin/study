let items1 = []
let items2 = [2;3;4;5;6;7]
let items3 = [1..5]
let items4 = [ for x in 1..5 do x ]

let extendedItems = 6::items4

let readList items =
    match items with
    | [] -> "Empty list"
    | [head] -> $"Head: {head}" // list containing one item
    | head::tail -> sprintf "Head %A and Tail %A" head tail

let emptyList = readList []
let multipleList = readList [1;2;3;4;5]
let singleItemList = readList [1]


let list1 = [1..5]
let list2 = [3..7]
let emptyList2 = []

let joined = list1 @ list2
let joinedEmpty = list1 @ emptyList2
let emptyJoined = emptyList2 @ list1

let joined2 = List.concat [list1;  list2]


let myList = [1..9]

let getEvents items =
    items
    |> List.filter (fun x -> x % 2 = 0)
    
let evens = getEvents myList

let sum items =
    items |> List.sum

let mySum = sum myList

let triple items =
    items
    |> List.map( fun x -> x * 3)

let myTriples = triple [1..5]

let print items =
    items
    |> List.iter (fun x -> (printfn "My value is %i" x))

print myList

let items5 = [(1, 0.25M);(5, 0.25M);(1, 2.25M);(1, 125M);(7, 10.9M)]

let sum2 items =
    items
    |> List.map (fun (q,p) -> decimal q * p)
    |> List.sum
    
let sum3 items =
    items
    |> List.sumBy (fun (q,p) -> decimal q*p)
    
    
// Folding
[1..10]
|> List.fold (fun acc v -> acc + v) 0

[1..10]
|> List.fold (+) 0

[1..10]
|> List.fold (*) 1

let items6 = [(1, 0.25M);(5, 0.25M);(1, 2.25M);(1, 125M);(7, 10.9M)]
let getTotal items =
    items
    |> List.fold (fun acc (q,p) -> acc + decimal q * p) 0M
    
let total = getTotal items6

let getTotal2 items =
    (0M, items) ||> List.fold (fun acc (q,p)-> acc + decimal q * p)
 
let total2 = getTotal2 items6

// Grouping data and Uniqueness