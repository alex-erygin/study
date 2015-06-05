using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor()
        {
            Receive<Messages.InputError>(x =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(x.Reason);
                    Console.ResetColor();
                });

            Receive<Messages.InputSuccess>(x =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(x.Reason);
                    Console.ResetColor();
                });

            ReceiveAny(x =>
                {
                    Console.WriteLine(x);
                    Console.ResetColor();
                });
        }
    }
}