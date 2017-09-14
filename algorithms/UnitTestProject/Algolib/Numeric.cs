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
        /// Вычислить наибольший общий делитель.
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
    }
}