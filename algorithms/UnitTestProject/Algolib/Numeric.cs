using System;
using System.Collections.Generic;

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
            
            var nextPrime = 3;
            var stopAt = Math.Sqrt(n);
            while (nextPrime <= stopAt)
            {
                // Исключаем числа, кратные данному простому числу.
                for (var i = nextPrime * 2; i < n; i+= nextPrime)
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

        
        /// <summary>
        /// Вычислить площадь по формуле прямоугольников. 
        /// </summary>
        /// <param name="function">Функция кривой.</param>
        /// <param name="xMin">Начальное значение x.</param>
        /// <param name="xMax">Конечное значение x.</param>
        /// <param name="numIntervals">Количество интервалов (чем больше, тем точнее).</param>
        /// <returns>Площадь прямоугольника.</returns>
        public static float UseRectangleRule(Func<float, float> function, float xMin, float xMax, int numIntervals)
        {
            float dx = (xMax - xMin) / numIntervals;

            float totalArea = 0;
            float x = xMin;

            for (int i = 1; i <= numIntervals; i++)
            {
                totalArea = totalArea + dx * function(x);
                x = x + dx;
            }

            return totalArea;
        }

        /// <summary>
        /// Вычислить площадь по формуле трапеции. 
        /// </summary>
        /// <param name="function">Функция кривой.</param>
        /// <param name="xMin">Начальное значение x.</param>
        /// <param name="xMax">Конечное значение x.</param>
        /// <param name="numIntervals">Количество интервалов (чем больше, тем точнее).</param>
        /// <returns>Площадь прямоугольника.</returns>
        public static float UseTrapezoidRule(Func<float, float> function, float xMin, float xMax, int numIntervals)
        {
            float dx = (xMax - xMin) / numIntervals;

            float totalArea = 0;
            float x = xMin;

            for (int i = 1; i <= numIntervals; i++)
            {
                totalArea = totalArea + dx * (function(x) + function(x + dx)) / 2;
                x = x + dx;
            }

            return totalArea;
        }
    }
}