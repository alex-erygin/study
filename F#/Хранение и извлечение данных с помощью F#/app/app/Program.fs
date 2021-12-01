

// For more information see https://aka.ms/fsharp-console-apps
printfn "Вводи значение, %%username%%"

let str = System.Console.ReadLine()

printfn "%s, %%username%%" str

printfn "Сколько тебе лет, %%username%%?" 

let ageString = System.Console.ReadLine()
let age = int ageString

printf "Тебе %i лет, старина" age