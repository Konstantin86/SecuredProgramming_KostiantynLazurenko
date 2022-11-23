using System;
using System.Linq;

namespace ConsoleApp5
{
    public static class Base32
    {
        private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public static string ToBase32String(this byte[] inBytes, bool padding = true)
        {
            if (inBytes == null || inBytes.Length == 0) 
                return "";

            var bits = inBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0'))
                .Aggregate((a, b) => a + b)
                .PadRight((int)(Math.Ceiling((inBytes.Length * 8) / 5d) * 5), '0');

            var result = Enumerable.Range(0, bits.Length / 5)
                .Select(i => ALLOWED_CHARS.Substring(Convert.ToInt32(bits.Substring(i * 5, 5), 2), 1))
                .Aggregate((a, b) => a + b); if (padding) { result = result.PadRight((int)(Math.Ceiling(result.Length / 8d) * 8), '='); }

            return result;
        }
    }
}