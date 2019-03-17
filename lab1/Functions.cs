using System;
using System.Collections;
using System.Linq;
namespace lab1
{
    class Functions
    {
        public static double ExpValue(IList series) //Среднее значение 
        {
            var m = (double)series[0] / series.Count;
            for (var i = 1; i < series.Count; i++)
                m += (double)series[i] / series.Count;
            return m;
        }   

        public static double Dispersion(IList series)
        {
            var m = ExpValue(series);
            var sum = series.Cast<object>().Sum(item => Math.Pow((double)item - m, 2));
            return sum / series.Count;
        }
        public static int[] Density(IEnumerable series) //Плотность распределения 
        {
            var density = new int[10];
            foreach (var number in series)
            {
                if ((double)number <= 0.1)
                    density[0]++;
                if ((double)number > 0.1 && (double)number <= 0.2)
                    density[1]++;
                if ((double)number > 0.2 && (double)number <= 0.3)
                    density[2]++;
                if ((double)number > 0.3 && (double)number <= 0.4)
                    density[3]++;
                if ((double)number > 0.4 && (double)number <= 0.5)
                    density[4]++;
                if ((double)number > 0.5 && (double)number <= 0.6)
                    density[5]++;
                if ((double)number > 0.6 && (double)number <= 0.7)
                    density[6]++;
                if ((double)number > 0.7 && (double)number <= 0.8)
                    density[7]++;
                if ((double)number > 0.8 && (double)number <= 0.9)
                    density[8]++;
                if ((double)number > 0.9 && (double)number <= 1)
                    density[9]++;
            }
            return density;
        }

        public static double Frequency(IList series) //Частотное распределение 
        {
            var m = ExpValue(series);
            var o = StdDev(series);
            var count = series.Cast<object>().Count(item => (double)item > m - o && (double)item < m + o);
            return (double)count * 100 / series.Count;
        }
        public static double StdDev(IList series) => Math.Sqrt(Dispersion(series)); //Поиск окрестностей

        public static int Correlation(ICollection series)
        {
            var number = new long[series.Count];
            var index = 0;
            foreach (var item in series)
            {
                number[index++] = Convert.ToInt64(item);
            }
            var correlationCount = 0;
            for (var i = 0; i <= number.Length / 2; i += 2)
            {
                while (number[i] != 0)
                {
                    if (((number[i] ^ number[i + 1]) & 1) == 1)
                    {
                        correlationCount++;
                    }
                    number[i] = number[i] >> 1;
                    number[i + 1] = number[i + 1] >> 1;
                }
            }
            return correlationCount;
        }
    }   

    /*Console.WriteLine($"MathStat: {Functions.MathStat(Ri) / Ri.Count:F4}");
            Console.WriteLine($"Correlation: {Functions.Correlation(series)}");
            Console.WriteLine("Density: ");
            var density = Functions.Density(Ri);
            for (i = 0; i < 10; i++)
            {
                Console.WriteLine(i < 9 ? $"0.{i} - 0.{i + 1}: {density[i]}" : $"0.{i} - 1.0: {density[i]}");
            }
            Console.WriteLine($"Frequency: {Functions.Frequency(Ri):F4}%");
            Console.WriteLine($"Dispersion: {Functions.Dispersion(Ri):F4}");
            Console.WriteLine($"Root-mean-square Deviation: {Math.Sqrt(Functions.Dispersion(Ri)):F4}"); */

}