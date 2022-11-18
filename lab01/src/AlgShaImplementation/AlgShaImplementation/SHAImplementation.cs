using System;
using System.Collections;
using System.Linq;
using AlgShaImplementation.Utils;

namespace AlgShaImplementation
{
    /// <summary>
    /// SHA1 algorythm implementation based on RFC3174: https://datatracker.ietf.org/doc/html/rfc3174
    /// </summary>
    class SHAImplementation
    {
        uint[] H = new uint[] { 0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };
        uint[] K = new uint[] { 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xCA62C1D6 }.SelectMany(k => Enumerable.Repeat(k, 20)).ToArray();
        uint[] W = new uint[80];

        private uint f(int t, uint B, uint C, uint D)
        {
            if (t < 20) return (B & C) | (~B & D);
            if (t < 40) return B ^ C ^ D;
            if (t < 60) return (B & C) | (B & D) | (C & D);
            return B ^ C ^ D;
        }

        private uint S(uint X, int n)
        {
            return (X << n) | (X >> (32 - n));
        }

        private void HashBlock(byte[] u8)
        {
            for (int i = 0; i < 80; i++)
            {
                    W[i] = (i < 16)
                        ? Convert.ToUInt32(u8[4 * i].ToHexString() + u8[4 * i + 1].ToHexString() + u8[4 * i + 2].ToHexString() + u8[4 * i + 3].ToHexString(), 16)
                        : S(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
            }

            uint a = H[0];
            uint b = H[1];
            uint c = H[2];
            uint d = H[3];
            uint e = H[4];

            for (int i = 0; i < 80; i++)
            {
                uint temp = (S(a, 5) + f(i, b, c, d) + e + W[i] + K[i]) % uint.MaxValue;

                e = d;
                d = c;
                c = S(b, 30);
                b = a;
                a = temp;
            }

            H[0] = (H[0] + a) % uint.MaxValue;
            H[1] = (H[1] + b) % uint.MaxValue;
            H[2] = (H[2] + c) % uint.MaxValue;
            H[3] = (H[3] + d) % uint.MaxValue;
            H[4] = (H[4] + e) % uint.MaxValue;
        }

        public string Hash(string messageText)
        {
            var msg = SetPadding(messageText.ToBitArray());
            
            for (int i = 0; i < msg.Count / 512; i++)
            {
                var blockMsg = new BitArray(512);
                for (int j = 0; j < 512; j++)
                    blockMsg[j] = msg[512 * i + j];

                bool tmp;
                for (int j = 0; j < 64; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        tmp = blockMsg[8 * j + k];
                        blockMsg[8 * j + k] = blockMsg[8 * (j + 1) - k - 1];
                        blockMsg[8 * (j + 1) - k - 1] = tmp;
                    }
                }

                var u8 = new Byte[64];
                blockMsg.CopyTo(u8, 0);
                HashBlock(u8);
            }

            return H[0].ToHexString() + H[1].ToHexString() + H[2].ToHexString() + H[3].ToHexString() + H[4].ToHexString();
        }

        private BitArray SetPadding(BitArray msg)
        {
            var originalMsgLength = msg.Count;

            msg.Length += 1;
            msg.Set(originalMsgLength, true);
            msg.Length = msg.Count > 448 ? msg.Length + (msg.Count - 448) % 512 == 0 ? 0 : 512 - (msg.Count - 448) % 512 : 448;

            var lengthBits = new BitArray(new[] { originalMsgLength });
            msg.Length += 64;
            for (int i = 0; i < 32; i++)
                msg[msg.Count - i - 1] = lengthBits[i];

            return msg;
        }
    }
}