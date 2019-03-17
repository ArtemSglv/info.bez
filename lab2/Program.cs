using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab2
{
    class Program
    {
        private static void Main()
        {
            const byte rounds = 5;
            Console.WriteLine("Enter key64 (0 for random): ");
            var key64 = Convert.ToUInt64(Console.ReadLine());
            if(key64.Equals(0))
                key64 = FeistelNet.RandomKey();
            Console.WriteLine($"Key64 {key64:X}");

            Console.Write("Message: ");
            var message = Console.ReadLine();
            var en_message = FeistelNet.Encrypt(message, rounds, key64);
            FeistelNet.Decrypt(en_message, key64, rounds);

            en_message = FeistelNet.EncryptCBC(message, rounds, key64);
            FeistelNet.DecryptCBC(message, key64, rounds);
        }
    }
}
