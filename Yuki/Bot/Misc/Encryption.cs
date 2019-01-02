using System;
using System.Security.Cryptography;
using System.Text;

namespace Yuki.Bot.Misc
{
    public class Encryption
    {
        public static string Encrypt(string data)
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(data), null, DataProtectionScope.CurrentUser));
        }
        
        public static string Decrypt(string data)
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(data), null, DataProtectionScope.CurrentUser));
        }
    }
}