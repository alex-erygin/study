using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace UnitTestProject.Algolib
{
    /// <summary>
    /// Реализация численных алгоритмов.
    /// </summary>
    public class Numeric
    {
        /// <summary>
        /// Возвести число а в степень N.
        /// </summary>
        public static int Power(int a, int n)
        {
            if (n == 0)
                return 1;

            if (n % 2 == 1)
                return Power(a, n - 1) * a;

            var b = Power(a, n / 2);
            return b * b;
        }

        /// <summary>
        /// Вычислить наибольший общий делитель ( O ( log(N) ) ).
        /// </summary>
        public static int Nod(int a, int b)
        {
            while (b != 0)
            {
                var r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        /// <summary>
        /// Разложить число на простые множители ( O(Sqrt(N)) ).
        /// </summary>
        public static List<int> FindFactors(int number)
        {
            var factors = new List<int>();
            while (number % 2 == 0)
            {
                factors.Add(2);
                number = number / 2;
            }

            var i = 3;
            var maxFactor = Math.Sqrt(number);
            while (i <= maxFactor)
            {
                while (number % i == 0)
                {
                    factors.Add(i);
                    number = number / i;
                    maxFactor = Math.Sqrt(number);
                }

                i += 2;
            }
            
            if (number > 1)
                factors.Add(number);

            return factors;
        }

        /// <summary>
        /// Найти простые числа от 1 до n.
        /// </summary>
        public static List<int> FindPrimes(int n)
        {
            var isComposite = new bool[n + 1];

            // сключаем числа, кратные 2.
            for (var i = 4; i < n; i += 2)
            {
                isComposite[i] = true;
            }
            
            int nextPrime = 3;
            var stopAt = Math.Sqrt(n);
            while (nextPrime <= stopAt)
            {
                // Исключаем числа, кратные данному простому числу.
                for (int i = nextPrime * 2; i < n; i+= nextPrime)
                {
                    isComposite[i] = true;
                }

                nextPrime += 2;

                while (nextPrime <= n && isComposite[nextPrime])
                {
                    nextPrime += 2;
                }
            }

            var primes = new List<int> {1};
            for (var i = 2; i < n; i++)
            {
                if (isComposite[i])
                    continue;
                primes.Add(i);
            }
            
            return primes;
        }
    }
}