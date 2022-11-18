using System;
namespace AlgShaImplementation.Utils
{
	public static class NumericExtenstions
	{
        public static string ToHexString(this uint number)
        {
            string result = @"";
            int i = 0;
            while (number != 0)
            {
                if (number % 16 < 10)
                {
                    result += (char)(number % 16 + 48);
                }
                else
                {
                    switch (number % 16)
                    {
                        case 10:
                            result += @"A";
                            break;
                        case 11:
                            result += @"B";
                            break;
                        case 12:
                            result += @"C";
                            break;
                        case 13:
                            result += @"D";
                            break;
                        case 14:
                            result += @"E";
                            break;
                        case 15:
                            result += @"F";
                            break;
                    }
                }
                number /= 16;
                i++;
            }
            while (i < 8)
            {
                result += @"0";
                i++;
            }
            String rresult = @"";
            for (i = 7; i >= 0; i--)
            {
                rresult += result[i];
            }
            return rresult;
        }

        public static string ToHexString(this byte @byte)
        {
            int H1 = @byte / 16;
            int H2 = @byte % 16;
            String result = @"";
            if (H1 < 10)
            {
                result += (char)(H1 + 48);
            }
            else
            {
                switch (H1)
                {
                    case 10:
                        result += @"A";
                        break;
                    case 11:
                        result += @"B";
                        break;
                    case 12:
                        result += @"C";
                        break;
                    case 13:
                        result += @"D";
                        break;
                    case 14:
                        result += @"E";
                        break;
                    case 15:
                        result += @"F";
                        break;
                }
            }
            if (H2 < 10)
            {
                result += (char)(H2 + 48);
            }
            else
            {
                switch (H2)
                {
                    case 10:
                        result += @"A";
                        break;
                    case 11:
                        result += @"B";
                        break;
                    case 12:
                        result += @"C";
                        break;
                    case 13:
                        result += @"D";
                        break;
                    case 14:
                        result += @"E";
                        break;
                    case 15:
                        result += @"F";
                        break;
                }
            }
            return result;
        }
    }
}

