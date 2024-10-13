using System;
using System.Security.Cryptography;
using System.Text;

namespace Services.Manager
{
    public static class PasswordGenerator
    {
        private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghjkmnpqrstuvwxyz";
        private const string Digits = "123456789";

        public static string GeneratePassword(int length = 12)
        {
            int requiredLength = length;

            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                while (password.Length < requiredLength)
                {
                    byte[] randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    int rand = BitConverter.ToInt32(randomBytes, 0) & int.MaxValue;

                    char nextChar = '\0';

                    if (password.Length < requiredLength)
                    {
                        if (password.Length == 0)
                            nextChar = Uppercase[rand % Uppercase.Length];
                        else if (password.Length == 1)
                            nextChar = Lowercase[rand % Lowercase.Length];
                        else if (password.Length == 2)
                            nextChar = Digits[rand % Digits.Length];
                        else
                            nextChar = GetRandomChar(rand);

                        password.Append(nextChar);
                    }
                }
            }

            return ShuffleString(password.ToString());
        }

        private static char GetRandomChar(int rand)
        {
            string allChars = Uppercase + Lowercase + Digits;
            return allChars[rand % allChars.Length];
        }

        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                int n = array.Length;
                while (n > 1)
                {
                    byte[] box = new byte[4];
                    rng.GetBytes(box);
                    int k = BitConverter.ToInt32(box, 0) % n;
                    if (k < 0) k += n;
                    n--;
                    char temp = array[k];
                    array[k] = array[n];
                    array[n] = temp;
                }
            }
            return new string(array);
        }
    }
}