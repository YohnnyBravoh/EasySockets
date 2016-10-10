using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Encryption
{
    public class RijndaelEncryption : Encryption
    {
        public RijndaelEncryption(string key) : base(key) { }

        /// <summary>
        /// Uses Rijndael to encrypt the data.
        /// </summary>
        /// <param name="data">The byte array that you want to encrypt.</param>
        /// <returns></returns>
        public override byte[] Encrypt(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var rijndael = new RijndaelManaged())
                {
                    byte[] keyData = GetKey();

                    rijndael.IV = keyData;
                    rijndael.Key = keyData;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Uses Rijndael to decrypt the data.
        /// </summary>
        /// <param name="data">The byte array that you want to decrypt.</param>
        /// <returns></returns>
        public override byte[] Decrypt(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var rijndael = new RijndaelManaged())
                {
                    byte[] keyData = GetKey();

                    rijndael.IV = keyData;
                    rijndael.Key = keyData;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, rijndael.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Converts the key into a byte array and then retrieves the MD5 hash as a byte array.
        /// </summary>
        /// <returns></returns>
        private byte[] GetKey()
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(_key));
            }
        }
    }
}
