using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public class SymmetricProvider<T>
        where T : SymmetricAlgorithm
    {
        public Encoding encode = Encoding.UTF8;
        public int bits = 2;

        protected SymmetricAlgorithm provider { get; set; }

        public SymmetricProvider()
        {
            var createMethod = typeof(T).GetMethod("Create", Type.EmptyTypes);
            provider = createMethod.Invoke(null, null) as T;
            provider.GenerateIV();
            provider.GenerateKey();
        }

        public SymmetricProvider(string iv, string key)
        {
            var createMethod = typeof(T).GetMethod("Create", Type.EmptyTypes);
            provider = createMethod.Invoke(null, null) as T;
            byte[] IV_Value = Convert.FromBase64String(iv);
            byte[] Key_Value = Convert.FromBase64String(key);
            provider.IV = IV_Value;
            provider.Key = Key_Value;
        }

        public string GetIV() => Convert.ToBase64String(provider.IV);

        public string GetKey() => Convert.ToBase64String(provider.Key);

        public string Encrypt(string stringToEncrypt)
        {
            byte[] inputByteArray = encode.GetBytes(stringToEncrypt);
            return Encrypt(inputByteArray);
        }

        public string Encrypt(byte[] byteToEncrypt)
        {
            byte[] inputByteArray = byteToEncrypt;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat($"{{0:X{bits}}}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        public string Decrypt(string stringToDecrypt)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length / bits];
            for (int x = 0; x < stringToDecrypt.Length / bits; x++)
            {
                int i = Convert.ToInt32(stringToDecrypt.Substring(x * bits, bits), 16);
                inputByteArray[x] = (byte)i;
            }
            //RijndaelManaged
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return encode.GetString(ms.ToArray());
        }
    }
}
