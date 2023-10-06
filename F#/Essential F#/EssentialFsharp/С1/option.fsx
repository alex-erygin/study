open System

let tryParseDateTime1 (input:string) =
    let (success, value) = DateTime.TryParse input
    if success then Some value else None
    
let tryParseDateTime2 (input:string) =
    match DateTime.TryParse input with
    | true, result -> Some result
    | _ -> None
    
let isDate = tryParseDateTime2 "2023-10-03"
let isNotDate = tryParseDateTime2 "zag-zag"