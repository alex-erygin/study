using System;
using System.Reactive.Linq;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            "Hello World^_^\0 this will be skipped"
                .ToObservable()
                .TakeWhile(x=>x != '\0')
                .Subscribe(Console.WriteLine);

            Console.WriteLine("Press da button to stop it");
            Console.ReadKey();
        }
    }
}