# Лабораторна робота №8 (реалізація механізму генерації одноразових паролів TOTP)

1. Результат роботи додатку с реалізацією TOTP генерації та генеруванням баркоду для інтеграції з Google Authenticator:
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/console_output.jpg?raw=true)

Як видно вище код 206938 який сгенеровано програмою Google Authenticator (після считування баркоду) було успішно провалідовано, але наступне використання того ж коду було неможливо тож ми отримали повідомлення "code is not valid!", наступний код, сгенерований Google Authenticator - 786561 було також провалідовано успішно.
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/otp_generator.png?raw=true)

# Складові частини
 - Для реалізації додутку обрана платформа та фреймворк .NET Core 3.1
 - За генерацію One time password відповідає клас TOTPAuthHelper та його метод CreateTOTP. Він реалізован згідно з наведеним зразком по генерації 6 значного коду (https://ru.wikipedia.org/wiki/HOTP)
  
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

 - Алгоритм генерації паролю реалізован за допомогою алгоритму перевірки цілостності HMAC та SHA1.
    private readonly HMACSHA1 hmac = new HMACSHA1(GenerateKey(20)); 
 - Для генерації баркоду в консолі використана бібліотека ZXing.NET, баркод генерується за допомогою коректної адреси otpauth протоколу
    public string GetUri(byte[] key, string account, string issuer)
    {
        return $"otpauth://totp/{Uri.EscapeUriString(issuer)}:{Uri.EscapeUriString(account)}?issuer={Uri.EscapeUriString(issuer)}" +
            $"&secret={ConsoleApp5.Base32.ToBase32String(key)}&algoruthm=SHA1&digits={charsCount}&period={intervalInSeconds}";
    }
