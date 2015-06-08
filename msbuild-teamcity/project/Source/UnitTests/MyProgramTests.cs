using ConsoleApp;
using Xunit;

namespace UnitTests
{
    public class MyProgramTests
    {
        [Fact]
        public void SayHello_NoException()
        {
            new MyProgram().SayHello();
        } 
    }
}