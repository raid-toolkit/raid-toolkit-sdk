using System.Security.Cryptography;
using System.Text;

namespace System;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (!string.IsNullOrEmpty(str) && str.Length > 1)
        {
            return $"{char.ToLowerInvariant(str[0])}{str.Substring(1)}";
        }
        return str;
    }
    public static string Sha256(this string value)
    {
        StringBuilder Sb = new();

        using (SHA256 hash = SHA256.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }
    public static int GetStableHashCode(this string str)
    {
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
