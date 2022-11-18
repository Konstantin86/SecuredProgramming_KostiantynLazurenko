using System;

namespace AlgShaImplementation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Please type in any string for SHA1 hashing...");

            var inputStr = Console.ReadLine();

            var sha1 = new SHAImplementation();

            var hash = sha1.Hash(inputStr);

            Console.WriteLine($"Message hash: {hash}\n");
            Console.WriteLine("Press 'Enter' to quit...");
            Console.ReadLine();
        }
    }
}