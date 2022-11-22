# Лабораторна робота №3 (реалізація алгоритму асиметричного шифрування RSI)

1. Результат роботи додатку с реалізацією алгоритму

dotnet run lab03
Please type in a message to encrypt with RSA
test Message!
Encrypted message t~0moHj>bK߫gD`l)wA(7\ (Bytes: 2,183,7,206,242,169,236,116,126,225,29,165,242,132,48,168,145,216,243,109,111,72,5,137,139,106,144,241,244,62,135,228,98,127,75,203,5,142,14,237,223,171,248,241,103,68,96,108,41,158,119,15,142,65,40,137,215,55,232,92,234,15,190,7)
Decrypted message: test Message! (Bytes: 116,101,115,116,32,77,101,115,115,97,103,101,33)
Press 'enter' to quit...

2. Реалізація виконувалась за допомогою наведеного стандарту описаного в RFC2313 - https://www.rfc-editor.org/rfc/rfc2313
3. Реалізація виконана на мові c# з використанням фреймворку .net core 3.1
4. Основу реалізації складає клас RsaProcessor з реалізацією алгоритмів шифрування та дешифрування та допоміжні функції. Додатково створені класси для опису RSI ключів та їх типів - RsiKey, RsiKeys та enum RsiKeyType.

# Алгоритм роботи додатку:
1. Юзер опитується щоб ввести тестове повідомленя для шифрування, яке далі трансформується в байтовий масив.
2. Далі генерується набів ключів RSI - приватний та публічний.
3. За допомогаю публічного ключа rsiProcessor шифрує повідомленя та віддає його як масив байтів.  
4. Шифроване повідомленя (та набір байт) відображаються кліенту в консолі
5. За допомогою приватного ключа зашифроване повідомлення розшифровується за допомогою об’єкта rsiProcessor
6. Дешифроване повідомлення (та набір байт) відображається клієнту в консолі аби користувач мав змогу перевірити правильність процесу

# Генерація ключів
1. Визначимо клас RsaKey, для зберігання RSA ключа відповідно до вимог RFC2313.
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

   PublicExponent згідно з рекомендацією RFC2313 стандартизуємо як число 65537. PrivateExponent буде містити розраховану приватну експоненту та Modulus - розраховане значення public modulus згідно з RFC (секція 6). Визначемо також допоміжні класи для зберігання пари ключів - RsaKeys та визначення типу ключа - RsaKeyType.
   
2. Алгоритми генерації ключів реалізуємо в окремому методі класу RsiProcessor згідно з інструкціями наведеними в 6 та 7 секціях RFC2313 - визначемо 2 числа prime за допомогою додаткових методів та modulus як їх множину (n = p*q), а приватну експоненту privateExponent таким чинов щоб d*e-1 ділилося на (p-1) та (q-1), де p та q - знайдені prime числа.
        public RsaKeys GenerateKeys(short len)
        {
            var prime1 = GeneratePrime((short)(len / 2));
            var prime2 = GeneratePrime((short)(len / 2));
            var modulus = prime1 * prime2;
            var privateExponent = Inverse(RsaKey.PublicExponent, (prime1 - 1) * (prime2 - 1));
            return new RsaKeys(modulus, privateExponent);
        }

# Шифрування повідомленя
1. Додамо до набору байт повідомлення додаткові 2 байту наприкінці (операція padding)
	    var alignedBytes = new byte[inputBytes.Length + 2];
            Array.Copy(inputBytes, alignedBytes, inputBytes.Length);
            alignedBytes[alignedBytes.Length - 1] = 0x00;
            alignedBytes[alignedBytes.Length - 2] = 0xFF;
2. Далі для отримання числового значення шифру скористуємось готовою функцією .NET BigInteger.ModPow() яка виконує модульне ділення числа, зведеного в ступінь іншого числа. Для цього передамо їй модифікований (в попередньому шагу) набір байт, публічну експоненту (константа),  та значення публічного ключа  Modulus.
3. Отриване число преобразуємо в набів байт та віддамо клієнту.
      var encyptedNum = BigInteger.ModPow(new BigInteger(alignedBytes), RsaKey.PublicExponent, publicKey.Modulus);
      return encyptedNum.ToByteArray();

# Дешифрування повідомленя
1. Дешифрування дещо схоже на шифрування але виконується в зворотньому порядку і з допомогою приватного ключа

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
