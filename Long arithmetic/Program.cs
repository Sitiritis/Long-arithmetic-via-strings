using System;

namespace Long_arithmetic
{
    class Program
    {
        static void Main(string[] args)
        {
            string factorial = Console.ReadLine();
            Console.WriteLine(fact(new MplbLongArithm(factorial)));

            Console.ReadKey();
        }

        public static MplbLongArithm fact(MplbLongArithm n)
        {
            MplbLongArithm one = new MplbLongArithm("1");
            return (n > one) ? fact(n - one) * n : one;
        }
    }
}
