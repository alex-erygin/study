using System;
using System.Linq.Expressions;
using Xunit;

namespace UnitTestProject
{
    public class RNG
    {
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
                var x = 0;
                for (var i = 1; i < n; i++)
                {
                    x = (A * xprev + B) % M;
                    xprev = x;                  
                }
            }
        }
    }
}