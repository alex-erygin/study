#if INTERACTIVE
#else
module JumpStart
#endif

let x = 42
let hi = "Hello"
let SayHiTo me = 
    printfn "Hi, %i" me

let Square = x = x*x

let Area (length:float, height:float) = 
    length * height