open System

let nullObj:string = null

let nullPri = Nullable<int>()

let fromNullObj = Option.ofObj nullObj
let fromNullPri = Option.ofNullable nullPri
let toNullObj = Option.toObj fromNullObj
let toNullPri = Option.toNullable fromNullPri

let resultDefaultValue = Option.defaultValue "---" fromNullObj
let resultFP = fromNullObj |> Option.defaultValue "---"

let setUnknownAsDefault = Option.defaultValue "???"

let result = setUnknownAsDefault fromNullObj

let resultPipe = fromNullObj |> setUnknownAsDefault

