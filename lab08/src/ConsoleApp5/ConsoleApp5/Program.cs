using System;
using ZXing.Common;
using static BarcodeHelper;

namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var strWriter = new StringWriter() { Fill = "  ", EmptyBlock = "\u2588\u2588", NewLine = "\n    ", };
            
            var authHelper = new TOTPAuthHelper();

            Console.WriteLine("\n{1}{0}{1}", 
                                RenderBarcode(new StringWriter() { Fill = "  ", EmptyBlock = "\u2588\u2588", NewLine = "\n    ", }, 
                                authHelper.GetUri("Test Account", "OTPGenerator"), new EncodingOptions { Height = 0, Width = 0, Margin = 1 }), strWriter.NewLine);

            while (true)
                Console.WriteLine(authHelper.VerifyOtp(Console.ReadLine()) ? "Code is valid!" : "Code is not valid");
        }
    }
}
