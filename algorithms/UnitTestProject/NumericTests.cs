using System;
using NUnit.Framework;
using UnitTestProject.Algolib;

namespace UnitTestProject
{
    /// <summary>
    /// Тесты для <see cref="Numeric"/>
    /// </summary>
    [TestFixture]
    public class NumericTests
    {
        [Test]
        [TestCase(12, 1200, 12)]
        public void NODTest(int a, int b, int expected)
        {
            var actual = Numeric.Nod(a, b);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(2, 16)]
        public void PowerTest(int a, int n)
        {
            var actual = Numeric.Power(a, n);
            var expected = Math.Pow(a, n);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(10, new[] { 2, 5 } )]
        [TestCase(123879874, new[] { 2, 47, 907, 1453 } )]
        public void FactorsTest(int n, int[] expected)
        {
            var actual = Numeric.FindFactors(n);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(10, new [] { 1, 2, 3, 5, 7 })]   
        public void FindPrimesTest(int n, int[] expectedPrimes)
        {
            var actual = Numeric.FindPrimes(n);
            Assert.AreEqual(expectedPrimes, actual);
        }
    }
}