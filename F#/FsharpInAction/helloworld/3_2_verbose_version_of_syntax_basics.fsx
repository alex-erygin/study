let addTenThenDouble (theNumber : int) : int = 
    let addTen : int = theNumber + 10;
    let answer : int = addTen * 2;
    printfn $"({theNumber} + 10) * 2 is {answer}";
    let website: System.Uri = new System.Uri ("https://fsharp.org");
    answer;
