# Лаборатобна робота №1: Гешування

# Мета роботи: 
* Дослідити існуючі механізми гешування. Реалізувати алгоритм гешування SHA (будь-якої версії).

# Хід роботи:
* Для реалізації алгоритму гешування SHA-1 звернемось до офіційного технічного опису роботи алгоритму https://datatracker.ietf.org/doc/html/rfc3174
* Для реалізації обрана мова c# та універсальна платформа .NET Core 3.1

1. Створимо новий Console проєкт в Visual Studio 2022
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab01/vs_project.png?raw=true)
2. Основу реалізації алгоритму зробимо в окремому классі - SHAImplementation
3. Реаулізуємо організацію вводу строки для гешування

Console.WriteLine("Hello! Please type in any string for SHA1 hashing...");
var inputStr = Console.ReadLine();
   
4. Далі реалізуємо алгоритм SHA-1

5. По-перше нам потрібно перевести строку для гешування в набір бітів для подальшого маніпулування з ними в алгоритмі. Зробимо окремий метод для цього:

        public static BitArray ToBitArray(this string str)
        {
            var arr = new BitArray(str.Select(c => (Byte)c).ToArray());

            bool t;

            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    t = arr[8 * i + j];
                    arr[8 * i + j] = arr[8 * (i + 1) - j - 1];
                    arr[8 * (i + 1) - j - 1] = t;
                }
            }

            return arr;
        }
6.  Далі, потрібно зробити вхідне повідомлення (бітову послідовність) кратне 512, розбити на блоки. Зробимо для цього окремий метод SetPadding:

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

Спочатку додамо біт із значенням 1 в кінець повідомлення.
А потім додаємо нульові (0) біти, їх кількість залежить від довжини повідомлення (448 - довжина повідомлення + 1)
В кінель додаємо 64 резервних біта (2 слова) значення яких буде відповідати довжині повідомлення.

7. Далі згідно с RFC напишему реалізацію самого алгорітму хешування

Гешування відбувається з використанням допоміжних константніх слів (8 байтніх послідовностей):
        uint[] H = new uint[] { 0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0 };
        uint[] K = new uint[] { 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xCA62C1D6 }.SelectMany(k => Enumerable.Repeat(k, 20)).ToArray();
        uint[] W = new uint[80];


Кожен з 512 бітних блоків проходить через функцію гешування за допомогаю 2х буферів з 5 слів та послідовності з 80 слів. Перший буфер це послідовність слів A, B, C, D, E, другий - H0, H1, H2, H3, H4:
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

8. Запустимо додаток та перевіримо працездатність
dotnet run AlgShaImplementation                     
Hello! Please type in any string for SHA1 hashing...
hello world
Message hash: 2AAE6C35C94FCFB415DBE95F408B9CE91EE846ED

9. Перевіримо результат з використанням утіліти openssl:
echo -n "hello world" | openssl sha1
2aae6c35c94fcfb415dbe95f408b9ce91ee846ed

![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab01/verification.png?raw=true)

# Висновки:
За результатом лабороторної роботи було досліджено та реалізовано механізми гешування SHA1.