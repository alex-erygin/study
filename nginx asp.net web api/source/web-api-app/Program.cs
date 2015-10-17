using System;

namespace Eon.Kiosk.Service
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            new SingleInstanceApplicationWrapper().Run(args);
        }
    }
}