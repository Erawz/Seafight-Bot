using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoxyBot.Licensing
{
    class Crypt
    {
        public static String Encrypt(string message, string key, string iv)
        {
            byte[] Key = Encoding.UTF8.GetBytes(key);
            byte[] IV = Encoding.UTF8.GetBytes(iv);

            string encrypted = null;
            RijndaelManaged rj = new RijndaelManaged
            {
                BlockSize = 256,
                KeySize = 256,
                Padding = PaddingMode.Zeros,
                IV = IV,
                Key = Key,
                Mode = CipherMode.CBC
            };

            MemoryStream ms = new MemoryStream();

            using (CryptoStream cs = new CryptoStream(ms, rj.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(message);
                    sw.Close();
                }
                cs.Close();
            }
            byte[] encoded = ms.ToArray();
            encrypted = Convert.ToBase64String(encoded);

            ms.Close();
            rj.Clear();
            return encrypted;
        }

        public static String Decrypt(string message, string key, string iv)
        {
            byte[] Key = Encoding.ASCII.GetBytes(key);
            byte[] IV = Encoding.ASCII.GetBytes(iv);
            var rijndael = new RijndaelManaged
            {
                BlockSize = 256,
                KeySize = 256,
                Padding = PaddingMode.Zeros,
                IV = IV,
                Key = Key,
                Mode = CipherMode.CBC
            };

            var buffer = Convert.FromBase64String(message);
            var transform = rijndael.CreateDecryptor();
            string decrypted;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                    cs.FlushFinalBlock();
                    decrypted = Encoding.UTF8.GetString(ms.ToArray());
                    cs.Close();
                }
                ms.Close();
            }
            return decrypted;
        }
    }
}
