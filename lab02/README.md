# Лабораторна робота №2 (реалізація алгоритму симетричного шифрування AES128)

# Мета роботи: 
* Дослідити симетричний алгоритм кодування AES128.
  
# Хід роботи:
* Для реалізації алгоритму гешування AES128 звернемось до офіційного технічного опису роботи алгоритму https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf
* Для реалізації обрана мова c# та універсальна платформа .NET Core 3.1

1. Створимо новий Console проєкт в Visual Studio 2022
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab01/vs_project.png?raw=true)
1. Основу реалізації алгоритму зробимо в окремому классі - Aes128Implementation
2. Реаулізуємо організацію вводу строки та ключа для кодування

            Console.WriteLine("Hello! Please type in any 16 bytes long message to encrypt with AES128...");
            var msg = Console.ReadLine();

            Console.WriteLine("Please type in any 16 bytes long phrase to be used as a cypher key...");
            var phrase = Console.ReadLine();
   
3. Далі реалізуємо алгоритм кодування та декодування використовуючи специфікацію https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf

        public byte[] Encrypt(string msg, string cypherPhrase)
        {
            var inputState = Encoding.ASCII.GetBytes(msg);
            var cypherKey = Encoding.ASCII.GetBytes(cypherPhrase);

            var outState = inputState.Select(s => s).ToArray();
            var k = cypherKey.Select(k => k).ToArray();

            for (int i = 0; i < 10; i++)
            {
                SubBytesTransform(outState, k);
                ShiftRowsTransformation(outState);
                if (i < 9) MixColumnTransform(outState);
                AddRoundKey(k, RoundConstant[i]);
            }

            AddLastRoundKey(outState, k);

            return outState;
        }

        public byte[] Decrypt(byte[] encodedState, byte[] cipherKey)
        {
            var outState = encodedState.Select(s => s).ToArray();
            var k = cipherKey.Select(k => k).ToArray();

            for (int i = 0; i< 10; i++)
                AddRoundKey(k, RoundConstant[i]);

            AddLastRoundKey(outState, k);

            for (int i = 0; i < 10; i++)
            {
                AddRoundKey(k, RoundConstant[9 - i], true);
                if (i > 0) MixColumnTransform(outState, true);
                ShiftRowsTransformation(outState, true);
                SubBytesTransform(outState, k, true);
            }

            return outState;
        }

5. Окремі шаги алгоритму виглядають наступним чином:
        private void SubBytesTransform(byte[] s, byte[] k, bool inverse = false)
        {
            for (int i = 0; i < 16; i++)
                s[i] = inverse ? (byte)(InverseSBox[s[i]] ^ k[i]) : SBox[s[i] ^ k[i]];
        }

        private void ShiftRowsTransformation(byte[] s, bool inverse = false)
        {
            int dimension = 4;

            var t1 = s[inverse ? dimension * 3 + 1 : 1];
            s[inverse ? dimension*3 + 1 : 1] = s[inverse ? dimension*2 + 1 : dimension + 1];
            s[inverse ? dimension * 2 + 1 : dimension + 1] = s[inverse ? dimension + 1 : dimension*2 + 1];
            s[inverse ? dimension + 1 : dimension * 2 + 1] = s[inverse ? 1 : dimension * 3 + 1];
            s[inverse ? 1 : dimension * 3 + 1] = t1;

            t1 = s[inverse ? dimension * 2 + 2 : 2];
            var t2 = s[inverse ? dimension * 3 + 2 : dimension + 2];
            s[inverse ? dimension * 2 + 2 : 2] = s[inverse ? 2 : dimension * 2 + 2];
            s[inverse ? dimension * 3 + 2 : dimension + 2] = s[inverse ? dimension + 2 : dimension * 3 + 2];
            s[inverse ? 2 : dimension * 2 + 2] = t1;
            s[inverse ? dimension  + 2 : dimension * 3 + 2] = t2;

            t1 = s[inverse ? 3 : dimension * 3 + 3];
            s[inverse ? 3 : dimension * 3 + 3] = s[inverse ? dimension + 3 : dimension * 2 + 3];
            s[inverse ? dimension + 3 : dimension * 2 + 3] = s[inverse ? dimension * 2 + 3 : dimension + 3];
            s[inverse ? dimension * 2 + 3 : dimension + 3] = s[inverse ? dimension * 3 + 3 : 3];
            s[inverse ? dimension * 3 + 3 : 3] = t1;
        }

        private void AddRoundKey(byte[] k, byte roundConstant, bool inverse = false)
        {
            if (inverse)
            {
                for (int i = 15; i > 3; i--)
                    k[i] = (byte)(k[i] ^ k[i - 4]);
            }

            k[0] = (byte)(SBox[k[13]] ^ k[0] ^ roundConstant);
            k[1] = (byte)(SBox[k[14]] ^ k[1]);
            k[2] = (byte)(SBox[k[15]] ^ k[2]);
            k[3] = (byte)(SBox[k[12]] ^ k[3]);

            if (!inverse)
            {
                for (int i = 4; i < 16; i++)
                    k[i] = (byte)(k[i] ^ k[i - 4]);
            }
        }

        private void MixColumnTransform(byte[] outState, bool decrypt = false)
        {
            byte t1, t2, t3, t4;

            for (int i = 0; i < 4; i++)
            {
                t4 = (byte)(i << 2);

                if (decrypt)
                {
                    t1 = Multiply(Multiply((byte)(outState[t4] ^ outState[t4 + 2])));
                    t2 = Multiply(Multiply((byte)(outState[t4 + 1] ^ outState[t4 + 3])));
                    outState[t4] ^= t1; outState[t4 + 1] ^= t2; outState[t4 + 2] ^= t1; outState[t4 + 3] ^= t2;
                }

                t1 = (byte)(((outState[t4] ^ outState[t4 + 1]) ^ outState[t4 + 2]) ^ outState[t4 + 3]);
                t2 = outState[t4];
                t3 = (byte)(outState[t4] ^ outState[t4 + 1]); t3 = Multiply(t3); outState[t4] = (byte)((outState[t4] ^ t3) ^ t1);
                t3 = (byte)(outState[t4 + 1] ^ outState[t4 + 2]); t3 = Multiply(t3); outState[t4 + 1] = (byte)((outState[t4 + 1] ^ t3) ^ t1);
                t3 = (byte)(outState[t4 + 2] ^ outState[t4 + 3]); t3 = Multiply(t3); outState[t4 + 2] = (byte)((outState[t4 + 2] ^ t3) ^ t1);
                t3 = (byte)(outState[t4 + 3] ^ t2); t3 = Multiply(t3); outState[t4 + 3] = (byte)((outState[t4 + 3] ^ t3) ^ t1);
            }
        }

6. Результат роботи додатку с реалізацією алгоритму

dotnet run lab02
Hello! Please type in any 16 bytes long message to encrypt with AES128...
qwertyui12345678
Please type in any 16 bytes long phrase to be used as a cypher key...
1 2 3 4 5 6 7 8 
Encoded Message: N?(e?4p?j?"??xU (bytes: 78,138,40,101,235,52,2,112,147,106,255,34,218,193,120,85)


Decoded Message: qwertyui12345678 (bytes: 113,119,101,114,116,121,117,105,49,50,51,52,53,54,55,56)

Press 'Enter' to quit...

7. Фінальний результат перевірено за допомогою онлайн сервісу https://encode-decode.com/aes128-encrypt-online/
