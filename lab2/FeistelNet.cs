using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab2
{
    class FeistelNet
    {
        public static string Encrypt(string message, uint rounds, ulong key64)
        {
            var mesgC = ToBlocks(Padding(message)); // массив блоков

            Console.WriteLine("Encrypted: ");
            for (var i = 0; i < mesgC.Length; i++)
            {
                mesgC[i] = Encrypt(mesgC[i], key64, rounds);
                //Console.WriteLine($"en_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message:X}");

            return message;
        }
        public static string Decrypt(string message, ulong key64, int rounds)
        {
            var mesgC = ToBlocks(message); // массив блоков
            //key64 = 2222221;

            Console.WriteLine("Decrypted: ");
            for (var i = 0; i < mesgC.Length; i++)
            {
                mesgC[i] = Decrypt(mesgC[i], key64, rounds);
                //Console.WriteLine($"dec_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message}");

            return message;
        }

        #region CBC
        static ulong in_vector = 111111;
        static ulong previousBlock = 0;
        public static string EncryptCBC(string message, uint rounds, ulong key64)
        {
            var mesgC = ToBlocks(Padding(message)); // массив блоков

            Console.WriteLine("Encrypted CBC: ");
            mesgC[0] = Encrypt(mesgC[0] ^ in_vector, key64, rounds);
            previousBlock = mesgC[0];
            for (var i = 1; i < mesgC.Length; i++)
            {
                mesgC[i] = Encrypt(mesgC[i] ^ previousBlock, key64, rounds);
                previousBlock = mesgC[i];
                //Console.WriteLine($"en_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message:X}");

            return message;
        }
        public static string DecryptCBC(string message, ulong key64, int rounds)
        {
            var mesgC = ToBlocks(message); // массив блоков

            Console.WriteLine("Decrypted CBC: ");
            previousBlock = mesgC[0];
            mesgC[0] = Decrypt(mesgC[0], key64, rounds) ^ in_vector;
            for (var i = 1; i < mesgC.Length; i++)
            {
                var tmp = mesgC[i];
                mesgC[i] = Decrypt(mesgC[i], key64, rounds) ^ previousBlock;
                previousBlock = tmp;
                //Console.WriteLine($"dec_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message}");

            return message;
        }
        #endregion

        #region CFB
        static ulong c_previousBlock = 111111;
        public static string EncryptCFB(string message, uint rounds, ulong key64)
        {
            var mesgC = ToBlocks(Padding(message)); // массив блоков

            Console.WriteLine("Encrypted CBC: ");
            for (var i = 0; i < mesgC.Length; i++)
            {
                mesgC[i] = Encrypt(mesgC[i] ^ previousBlock, key64, rounds);
                previousBlock = mesgC[i];
                //Console.WriteLine($"en_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message:X}");

            return message;
        }
        public static string DecryptCFB(string message, ulong key64, int rounds)
        {
            var mesgC = ToBlocks(message); // массив блоков

            Console.WriteLine("Decrypted CBC: ");
            for (var i = 0; i < mesgC.Length; i++)
            {
                mesgC[i] = Decrypt(mesgC[i], key64, rounds);
                //Console.WriteLine($"dec_block {i}: {mesgC[i]:X}");
            }
            message = MessageToString(mesgC);
            Console.WriteLine($"{message:X}");

            return message;
        }
        #endregion

        /// <summary>
        /// Дополнение строки до кратности 64м
        /// </summary>
        private static string Padding(string input)
        {
            var n = input.Length * 16 % 64; // n полных блоков
            if (n == 0) return input;
            var sb = new StringBuilder(input);
            var k = (64 - n) / 16;
            sb.Append(new char(), k);
            return sb.ToString();
        }

        ///<summary>
        /// Разделение на сообщения на массив блоков по 64 бита
        ///</summary>
        private static ulong[] ToBlocks(string input)
        {
            // так как символ по 16 бит, а нам нужно составить блок из 64 бит..
            var result = new ulong[input.Length / 4];
            var temp = new uint[2];
            for (int i = 0, j = 0; i < input.Length; i += 4, j++)
            {
                temp[0] = (uint)input[i] << 16 | input[i + 1];
                temp[1] = (uint)input[i + 2] << 16 | input[i + 3];

                result[j] = (ulong)temp[0] << 2 * 16 | temp[1];
            }
            return result;
        }

        ///<summary>
        /// Генерация рандомного 64 битного ключа шифрования
        ///</summary>
        public static ulong RandomKey()
        {
            var rand = new Random((int)(DateTime.Now.Ticks & 0xFFFFFFFF));
            var buffer = new byte[sizeof(ulong)];
            rand.NextBytes(buffer);
            ulong res = 0;
            for (var i = 0; i < sizeof(ulong); i++)
            {
                ulong temp = buffer[i];
                temp = temp << 8 * (7 - i);
                res = res | temp;
            }
            return res;
        }

        private static ulong Encrypt(ulong msg, ulong key64, uint rounds)
        {
            var right = (uint)(msg << 2 * 16 >> 2 * 16);
            var left = (uint)(msg >> 2 * 16);
            for (var i = 0; i < rounds; i++)
            {
                var key32I = KeyGenerator(i, key64);
                //Console.WriteLine($"key32 {i}: {key32I:X}");
                var function = F(left, key32I);
                var tmp = left;
                left = right ^ function;
                right = tmp;
            }
            var tmp1 = (ulong)left << 2 * 16;
            var tmp2 = (ulong)right;
            return tmp1 | tmp2;
        }

        private static ulong Decrypt(ulong msg, ulong key64, int rounds)
        {
            var right = (uint)(msg << 2 * 16 >> 2 * 16);
            var left = (uint)(msg >> 2 * 16);
            for (var i = rounds - 1; i >= 0; i--)
            {
                var key32I = KeyGenerator(i, key64);
                //Console.WriteLine($"key32 {i}: {key32I:X}");
                var function = F(right, key32I);
                var tmp = right;
                right = left ^ function;
                left = tmp;
            }
            var tmp1 = (ulong)left << 2 * 16;
            var tmp2 = (ulong)right;
            return tmp1 | tmp2;
        }
        private static string MessageToString(IEnumerable<ulong> msg)
        {
            var result = string.Empty;
            var tmp = new ushort[4];
            foreach (var item in msg)
            {
                tmp[0] = (ushort)(item >> 3 * 16);
                tmp[1] = (ushort)(item >> 2 * 16 << 3 * 16 >> 3 * 16);
                tmp[2] = (ushort)(item << 2 * 16 >> 3 * 16);
                tmp[3] = (ushort)(item << 3 * 16 >> 3 * 16);
                result = tmp.Aggregate(result, (current, t) => current + Convert.ToChar(t));
            }
            return result;
        }

        #region cycleMove
        private static ulong CycleMoveRight(ulong number, byte offset) => number >> offset | number << 64 - offset;

        private static uint CycleMoveRight(uint number, byte offset) => number >> offset | number << 32 - offset;

        private static uint CycleMoveLeft(uint number, byte offset) => number << offset | number >> 32 - offset;
        #endregion

        private static uint F(uint left, uint key) => CycleMoveLeft(left, 9) ^ ~(CycleMoveRight(key, 11) + left);

        private static uint KeyGenerator(int round, ulong key64) => (uint)(CycleMoveRight(key64, (byte)(round * 8)));
    }
}
