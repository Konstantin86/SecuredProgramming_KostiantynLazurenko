using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class TOTPAuthHelper
{
    private const int charsCount = 6;
    private const int intervalInSeconds = 30;

    private readonly HMACSHA1 hmac = new HMACSHA1(GenerateKey(20));
    private List<string> usedCodes = new List<string>();

    public bool VerifyOtp(string code)
    {
        if (code == null || code.Length != charsCount || usedCodes.Contains(code)) return false;
        var state = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / intervalInSeconds) - 1;
        
        for (int i = 0; i <= 2; i++)
        {
            usedCodes.Add(code);
            if (CreateTOTP(state: state + i) == code) return true;
        }

        return false;
    }

    private string CreateTOTP(long state)
    {
        var stateBytes = BitConverter.GetBytes(state);
        
        if (BitConverter.IsLittleEndian) 
            Array.Reverse(stateBytes);

        var stateBytesHash = hmac.ComputeHash(stateBytes);

        var offset = stateBytesHash[stateBytesHash.Length - 1] & 0xf;

        var binary = ((stateBytesHash[offset] & 0x7f) << 24) 
            | ((stateBytesHash[offset + 1] & 0xff) << 16) 
            | ((stateBytesHash[offset + 2] & 0xff) << 8) 
            | (stateBytesHash[offset + 3] & 0xff);

        return (binary % Math.Pow(10, charsCount)).ToString(new string('0', charsCount));
    }

    public static byte[] GenerateKey(int length)
    {
        using (var randNumGen = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            randNumGen.GetBytes(bytes);
            return bytes;
        }
    }

    public string GetUri(byte[] key, string account, string issuer)
    {
        return $"otpauth://totp/{Uri.EscapeUriString(issuer)}:{Uri.EscapeUriString(account)}?issuer={Uri.EscapeUriString(issuer)}" +
            $"&secret={ConsoleApp5.Base32.ToBase32String(key)}&algoruthm=SHA1&digits={charsCount}&period={intervalInSeconds}";
    }

    public string GetUri(string accountName, string issuer) { return GetUri(hmac.Key, accountName, issuer); }
}