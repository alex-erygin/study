using System;

namespace ConsoleZog
{
    class Program
    {
        static void Main(string[] args)
        {
            //new AsyncBacgroundOperations().RunDemo();

            new ObservationOperations().RunDemo();

            Console.WriteLine("Main thread Completed");
            Console.ReadKey();
        }
    }
}