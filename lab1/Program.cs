using System;
using System.Collections.Generic;
using System.Linq;

namespace lab1
{
    class Program
    {
        private static void Main()
        {
            //Un+1 = (M*Un+c) mod p
            var series = new List<double> { 2 };
            const int p = 18439; //18439
            var Ri = new List<double> { series[0] / (double)p };
            const int C = 1; //приращение 
            const int M = 16807; //множитель
            var i = 0;

            // while (series.Count < p)
            // {
            //     var nextNumber = (M * series[i++] + C) % p; 

            //     if (series.Contains(nextNumber))
            //     {                    
            //         if (series.Count < 10000)
            //         {
            //             Console.WriteLine("Повтор значений");
            //             break;
            //         }
            //     }

            //     series.Add(nextNumber);
            //     Ri.Add(nextNumber / (double)p);
            // }

            while (series.Count < p)
            {
                var nextNumber = (M * series[i++] + C) % p;    

                if (series.Contains(nextNumber))
                {            
                    Console.WriteLine($"Повтор значений, nextNumber: {nextNumber}");
                    break;
                }

                series.Add(nextNumber);
                Ri.Add(nextNumber / (double)p);

                
            }

            Console.WriteLine(series.Count);
            Console.WriteLine($"R0: {series.First()}");
            Console.WriteLine($"Rn: {series.Last()}");


            #region test
            
            Console.WriteLine($"ExpValue: {Functions.ExpValue(Ri):F4}");
            Console.WriteLine($"Dispersion: {Functions.Dispersion(Ri):F4}");
            
            #endregion
        }                
    }
}
