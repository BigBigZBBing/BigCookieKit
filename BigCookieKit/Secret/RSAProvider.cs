using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;

namespace BigCookieKit.Secret
{
    public class RSAProvider
    {
        private RSACryptoServiceProvider provider { get; set; }

        private CspParameters csp { get; set; }

        public static CspProviderFlags RSAFlags { get; set; } = CspProviderFlags.UseDefaultKeyContainer;

        public bool fOAEP { get; set; } = false;

        public Encoding Encode { get; set; } = Encoding.UTF8;

        public RSAProvider() : this(2048)
        {

        }

        public RSAProvider(int size)
        {
            csp = new CspParameters(1, "BigCookie Cryptographic Service Provider");
            csp.Flags = RSAFlags;
            csp.KeyContainerName = "BigCookie-RSA";
            provider = new RSACryptoServiceProvider(size, csp);
        }

        public RSAProvider(int size, string password) : this(size)
        {
            SecureString secure = new SecureString();
            password.ToList().ForEach(x => secure.AppendChar(x));
            csp.KeyPassword = secure;
        }

        public void SetPrivateKey(string pem)
        {
            //PemReader pr = new PemReader(new StringReader(pem));
            //AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            //RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);
            //provider.ImportParameters(rsaParams);
        }

        public void SetPublicKey(string pem)
        {
            //PemReader pr = new PemReader(new StringReader(pem));
            //AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            //RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);
            //provider.ImportParameters(rsaParams);
        }

        public string GetPublickKey()
        {
            StringWriter outputStream = new StringWriter();
            var parameters = provider.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // 序列号
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // 序列号
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // 对象标识符
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // 位串
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // 未使用的部分
                        bitStringWriter.Write((byte)0x30); // 序列号
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); 
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); 
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                    outputStream.Write("\n");
                }
                outputStream.Write("-----END PUBLIC KEY-----");
            }

            return outputStream.ToString();
        }

        public string GetPrivateKey()
        {
            StringWriter outputStream = new StringWriter();
            if (provider.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = provider.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                // WriteLine terminates with \r\n, we want only \n
                outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                    outputStream.Write("\n");
                }
                outputStream.Write("-----END RSA PRIVATE KEY-----");
            }

            return outputStream.ToString();
        }

        public string Encrypt(string text)
        {
            byte[] bytes = provider.Encrypt(Encode.GetBytes(text), fOAEP);
            return Convert.ToBase64String(bytes);
        }

        public string Dncrypt(string encrypt)
        {
            byte[] bytes = Convert.FromBase64String(encrypt);
            return Encode.GetString(provider.Decrypt(bytes, fOAEP));
        }

        public byte[] Encrypt(byte[] text)
        {
            byte[] bytes = provider.Encrypt(text, fOAEP);
            return bytes;
        }

        public byte[] Dncrypt(byte[] encrypt)
        {
            byte[] bytes = encrypt;
            return provider.Decrypt(bytes, fOAEP);
        }

        private void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }
    }
}
