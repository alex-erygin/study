using System.IO;
using System.Reflection;
using Infotecs.RedAlert;

namespace ConsoleApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var crgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "List.prg");
            IntegrityChecker.CheckIntegrity(crgPath);
            new MyProgram().SayHello();
        }
    }
}