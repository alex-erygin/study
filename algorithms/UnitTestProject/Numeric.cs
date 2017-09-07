using System;
using System.Linq.Expressions;
using Xunit;

namespace UnitTestProject
{
    public class Numeric
    {
        /// <summary>
        /// RNG -Линейный конгруэнтный генератор.
        /// </summary>
        [Fact]
        public void LinearCongruentGeterator()
        {
            const int n = 1000000;
            const int A = 11;
            const int B = 12;
            const int M = 140;
            using (new Benchmark($"Линейный конгруэнтный генератор для n = {n}"))
            {
                var xprev = 0;
                for (var i = 1; i < n; i++)
                {
                    var x = (A * xprev + B) % M;
                    xprev = x;
                }
            }
        }

        /// <summary>
        /// Наибольший общий делитель.
        /// </summary>
        [Fact]
        public void NOD()
        {
            var a = 158834732;
            var b = 343432;

            while (b != 0)
            {
                var r = a % b;
                a = b;
                b = r;
            }
            
            Console.WriteLine(a);
        }
    }
}