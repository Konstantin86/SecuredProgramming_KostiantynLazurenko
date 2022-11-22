using System;
using System.Numerics;
using System.Text;
using System.Linq;

namespace lab03
{
    public class RsaProcessor
    {
        public RsaKeys GenerateKeys(short len)
        {
            var prime1 = GeneratePrime((short)(len / 2));
            var prime2 = GeneratePrime((short)(len / 2));
            var modulus = prime1 * prime2;
            var privateExponent = Inverse(RsaKey.PublicExponent, (prime1 - 1) * (prime2 - 1));
            return new RsaKeys(modulus, privateExponent);
        }

        public BigInteger DefinePrime(short len)
        {
            var bytes = new byte[(len / 8) + 1];
            new Random(Environment.TickCount).NextBytes(bytes);
            bytes[bytes.Length - 1] = 0x0;
            bytes[0] = (byte)(1 << 0);
            bytes[bytes.Length - 2] = (byte)(1 << 7);
            bytes[bytes.Length - 2] = (byte)(1 << 6);

            while (true)
            {
                if (PrimeTest(new BigInteger(bytes), 40)) break;
                else
                {
                    bytes = (new BigInteger(bytes) + 2).ToByteArray();

                    var upper_limit = new byte[bytes.Length];
                    upper_limit[bytes.Length - 1] = 0x0;

                    if (new BigInteger(upper_limit) - 20 < new BigInteger(bytes) && new BigInteger(bytes) < new BigInteger(upper_limit))
                        return new BigInteger(-1);
                }
            }

            return new BigInteger(bytes);
        }

        public byte[] EncryptBytes(byte[] inputBytes, RsaKey publicKey)
        {
            var alignedBytes = new byte[inputBytes.Length + 2];
            Array.Copy(inputBytes, alignedBytes, inputBytes.Length);
            alignedBytes[alignedBytes.Length - 1] = 0x00;
            alignedBytes[alignedBytes.Length - 2] = 0xFF;

            var encyptedNum = BigInteger.ModPow(new BigInteger(alignedBytes), RsaKey.PublicExponent, publicKey.Modulus);

            return encyptedNum.ToByteArray();
        }

        public byte[] DecryptBytes(byte[] encBytes, RsaKey privateKey)
        {
            var originalNum = BigInteger.ModPow(new BigInteger(encBytes), privateKey.PrivateExponent, privateKey.Modulus);
            var originalBytes = originalNum.ToByteArray();

            int len = -1;
            for (int i = originalBytes.Length - 1; i >= 0; i--)
            {
                if (originalBytes[i] == 0xFF)
                {
                    len = i;
                    break;
                }
            }

            var result = new byte[len];
            Array.Copy(originalBytes, result, len);
            return result;
        }

        private BigInteger GeneratePrime(short l)
        {
            var prime = DefinePrime((short)(l / 2));
            while (prime % RsaKey.PublicExponent == 1) prime = DefinePrime((short)(l / 2));
            return prime;
        }

        private BigInteger Inverse(BigInteger u, BigInteger v)
        {
            BigInteger u1 = 1; BigInteger u3 = u; BigInteger v1 = 0; BigInteger v3 = v; BigInteger i = 1;

            while (v3 != 0)
            {
                var q = u3 / v3;
                var t3 = u3 % v3;
                var t1 = u1 + q * v1;
                u1 = v1; v1 = t1; u3 = v3; v3 = t3;
                i = -i;
            }

            return (u3 != 1) ? 0 : (i < 0) ? v - u1 : u1;
        }

        public static bool PrimeTest(BigInteger n, int k)
        {
            if (n == 2 || n == 3) return true;
            if (n < 2 || n % 2 == 0) return false;

            var d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            var bytes = new byte[n.ToByteArray().Length];
            BigInteger a;

            for (int i = 0; i < k; i++)
            {
                do
                {
                    new Random(Environment.TickCount).NextBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= n - 2);

                var x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1) return false;
                    else if (x == n - 1) break;
                }

                if (x != n - 1) return false;
            }

            return true;
        }
    }

    public class RsaKeys
    {
        public RsaKeys(RsaKey privateKey, RsaKey publicKey) { PrivateKey = privateKey; PublicKey = publicKey; }
        public RsaKeys(BigInteger modulus, BigInteger privateExponent) : this(new RsaKey(modulus, RsaKeyType.Private, privateExponent), new RsaKey(modulus)) { }

        public RsaKey PrivateKey { get; }
        public RsaKey PublicKey { get; }
    }

    public class RsaKey
    {
        public const int PublicExponent = 65537;

        public RsaKeyType Type { get; set; }
        public BigInteger Modulus { get; set; }
        public BigInteger PrivateExponent { get; }

        public RsaKey(BigInteger n, RsaKeyType type, BigInteger d)
        {
            Modulus = n;
            PrivateExponent = d;
            Type = type;
        }

        public RsaKey(BigInteger n)
        {
            Modulus = n;
            Type = RsaKeyType.Public;
        }
    }

    public enum RsaKeyType { Public, Private }
}