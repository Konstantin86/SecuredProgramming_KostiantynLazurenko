using System;
using Aes128;
using System.Text;

namespace lab02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Please type in any 16 bytes long message to encrypt with AES128...");
            var msg = Console.ReadLine();

            Console.WriteLine("Please type in any 16 bytes long phrase to be used as a cypher key...");
            var phrase = Console.ReadLine();

            Aes128Implementation aes128 = new Aes128Implementation();

            var key = Encoding.ASCII.GetBytes(phrase);
            byte[] enc = aes128.Encrypt(msg, phrase);
            var encMsg = System.Text.Encoding.ASCII.GetString(enc);

            Console.WriteLine($"Encoded Message: {encMsg} (bytes: {string.Join(",", enc)})\n");

            byte[] dec = aes128.Decrypt(enc, key);
            var decMsg = System.Text.Encoding.Default.GetString(dec);

            Console.WriteLine($"Decoded Message: {decMsg} (bytes: {string.Join(",", dec)})\n");
            Console.WriteLine("Press 'Enter' to quit...");
            Console.ReadLine();
        }
    }
}

