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
        /// <summary>
        /// Наибольший общий делитель.
        /// </summary>
        [Test]
        [TestCase(12, 1200, 12)]
        public void NOD(int a, int b, int expected)
        {
            var actual = Numeric.Nod(a, b);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// Возведение в степень.
        /// </summary>
        [Test]
        [TestCase(2, 16)]
        public void PowerTes(int a, int n)
        {
            var actual = Numeric.Power(a, n);
            var expected = Math.Pow(a, n);
            Assert.AreEqual(expected, actual);
        }
    }
}