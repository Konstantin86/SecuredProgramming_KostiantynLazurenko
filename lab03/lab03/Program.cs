using System;

namespace lab03
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please type in a message to encrypt with RSA");

            var msg = Console.ReadLine();

            var rsaProcessor = new RsaProcessor();
            var rsaKeys = rsaProcessor.GenerateKeys(1024);
            var msgBytes = System.Text.Encoding.UTF8.GetBytes(msg);
            var encBytes = rsaProcessor.EncryptBytes(msgBytes, rsaKeys.PublicKey);

            Console.WriteLine($"Encrypted message {System.Text.Encoding.UTF8.GetString(encBytes)} (Bytes: {string.Join(",", encBytes)})");

            var decBytes = rsaProcessor.DecryptBytes(encBytes, rsaKeys.PrivateKey);
            var decryptedMsg = System.Text.Encoding.UTF8.GetString(decBytes);

            Console.WriteLine($"Decrypted message: {decryptedMsg} (Bytes: {string.Join(",", decBytes)})\nPress 'enter' to quit...");
            Console.ReadLine();
        }
    }
}