using BigCookieKit.IO;
using BigCookieKit.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public class RSAProvider<T> : IDisposable
        where T : HashAlgorithm
    {
        private bool disposedValue;

        /// <summary>
        /// RSA提供者
        /// </summary>
        private RSACryptoServiceProvider provider { get; set; }

        /// <summary>
        /// 使用的Hash算法
        /// </summary>
        private HashAlgorithm algorithm { get; set; }

        /// <summary>
        /// 公钥参数
        /// </summary>
        private RSAParameters publickKey { get; set; }

        /// <summary>
        /// 私钥参数
        /// </summary>
        private RSAParameters privateKey { get; set; }

        /// <summary>
        /// Hash算法名称
        /// </summary>
        private string HashName => algorithm.HashSize switch
        {
            128 => "MD5",
            160 => "SHA1",
            256 => "SHA256",
            384 => "SHA384",
            512 => "SHA512",
            _ => throw new ArgumentOutOfRangeException()
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider">RSA提供者</param>
        public RSAProvider(RSACryptoServiceProvider provider = null)
        {
            this.provider = provider ?? new RSACryptoServiceProvider();
            var createMethod = typeof(T).GetMethod("Create", Type.EmptyTypes);
            this.algorithm = createMethod.Invoke(null, null) as T;
            this.publickKey = this.provider.ExportParameters(false);
            this.privateKey = this.provider.ExportParameters(true);
        }

        /// <summary>
        /// 导入RSA参数密钥
        /// </summary>
        /// <param name="paramter"></param>
        public void ImportParameter(RSAParameters paramter)
        {
            provider.ImportParameters(paramter);
        }

        /// <summary>
        /// 导入RSA的xml密钥
        /// </summary>
        /// <param name="xml"></param>
        public void ImportXml(string xml)
        {
            var xmlRead = new XmlReadKit(xml.ToStream());
            var packet = xmlRead.XmlRead("RSAKeyValue");
            RSAParameters paramter = default;
            paramter.D = GetNodeByte(packet, "D");
            paramter.DP = GetNodeByte(packet, "DP");
            paramter.DQ = GetNodeByte(packet, "DQ");
            paramter.Exponent = GetNodeByte(packet, "Exponent");
            paramter.InverseQ = GetNodeByte(packet, "InverseQ");
            paramter.Modulus = GetNodeByte(packet, "Modulus");
            paramter.P = GetNodeByte(packet, "P");
            paramter.Q = GetNodeByte(packet, "Q");
            provider.ImportParameters(paramter);
        }

        /// <summary>
        /// 根据Xml包获取子节点的字节流
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private byte[] GetNodeByte(XmlPacket packet, string node)
        {
            var _node = packet.Node.FirstOrDefault(x => x.Info.Name == node);
            if (_node != null && !string.IsNullOrEmpty(_node.Info.Text))
            {
                return Encoding.UTF8.GetBytes(_node.Info.Text);
            }
            return null;
        }

        /// <summary>
        /// 导入RSA的base64密钥
        /// </summary>
        /// <param name="secret"></param>
        public void ImportBase64(string secret)
        {

        }

        /// <summary>
        /// 导入RSA的pem密钥
        /// </summary>
        /// <param name="pem"></param>
        public void ImportPem(string pem)
        {

        }

        /// <summary>
        /// 获取公钥密钥
        /// </summary>
        /// <returns>
        /// <para>Item1:PublicKey</para>
        /// <para>Item2:PrivateKey</para>
        /// </returns>
        public Tuple<string, string> GetXmlSecret()
        {
            return Tuple.Create(provider.ToXmlString(false), provider.ToXmlString(true));
        }

        /// <summary>
        /// 获取PKCS#1公钥密钥
        /// </summary>
        /// <returns>
        /// <para>Item1:PublicKey</para>
        /// <para>Item2:PrivateKey</para>
        /// </returns>
        public Tuple<string, string> GetBase64Secret()
        {
            return Tuple.Create(Convert.ToBase64String(provider.ExportRSAPublicKey()), Convert.ToBase64String(provider.ExportRSAPrivateKey()));
        }

        public string ExportPrivateKey()
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

        public string ExportPublicKey()
        {
            StringWriter outputStream = new StringWriter();
            var parameters = provider.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
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
                // WriteLine terminates with \r\n, we want only \n
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

        private void EncodeLength(BinaryWriter stream, int length)
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

        /// <summary>
        /// 进行签名
        /// </summary>
        /// <param name="bytes">内容</param>
        /// <returns></returns>
        public string Sign(byte[] bytes)
        {
            byte[] hash = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(provider.SignHash(hash, CryptoConfig.MapNameToOID(HashName)));
        }

        /// <summary>
        /// 签名校验
        /// </summary>
        /// <param name="sign">签名</param>
        /// <param name="bytes">内容</param>
        /// <returns></returns>
        public bool Verify(string sign, byte[] bytes)
        {
            byte[] hash = algorithm.ComputeHash(bytes);
            return provider.VerifyHash(hash, CryptoConfig.MapNameToOID(HashName), Convert.FromBase64String(sign));
        }

        public RSAParameters getKeyPara(string hashKey, int type)
        {
            RSAParameters rsaP = new RSAParameters();
            byte[] tmpKeyNoB64 = Convert.FromBase64String(hashKey);
            int pemModulus = 128;
            int pemPublicExponent = 3;
            int pemPrivateExponent = 128;
            int pemPrime1 = 64;
            int pemPrime2 = 64;
            int pemExponent1 = 64;
            int pemExponent2 = 64;
            int pemCoefficient = 64;
            byte[] arrPemModulus = new byte[128];
            byte[] arrPemPublicExponent = new byte[3];
            byte[] arrPemPrivateExponent = new byte[128];
            byte[] arrPemPrime1 = new byte[64];
            byte[] arrPemPrime2 = new byte[64];
            byte[] arrPemExponent1 = new byte[64];
            byte[] arrPemExponent2 = new byte[64];
            byte[] arrPemCoefficient = new byte[64];

            if (type == 0)//私钥
            {
                //Modulus
                for (int i = 0; i < pemModulus; i++)
                {
                    arrPemModulus[i] = tmpKeyNoB64[11 + i];
                }
                rsaP.Modulus = arrPemModulus;

                //PublicExponent
                for (int i = 0; i < pemPublicExponent; i++)
                {
                    arrPemPublicExponent[i] = tmpKeyNoB64[141 + i];
                }
                rsaP.Exponent = arrPemPublicExponent;

                //PrivateExponent
                for (int i = 0; i < pemPrivateExponent; i++)
                {
                    arrPemPrivateExponent[i] = tmpKeyNoB64[147 + i];
                }
                rsaP.D = arrPemPrivateExponent;

                //Prime1
                for (int i = 0; i < pemPrime1; i++)
                {
                    arrPemPrime1[i] = tmpKeyNoB64[278 + i];
                }
                rsaP.P = arrPemPrime1;

                //Prime2
                for (int i = 0; i < pemPrime2; i++)
                {
                    arrPemPrime2[i] = tmpKeyNoB64[345 + i];
                }
                rsaP.Q = arrPemPrime2;

                //Exponent1
                for (int i = 0; i < pemExponent1; i++)
                {
                    arrPemExponent1[i] = tmpKeyNoB64[412 + i];
                }
                rsaP.DP = arrPemExponent1;

                //Exponent2
                for (int i = 0; i < pemExponent2; i++)
                {
                    arrPemExponent2[i] = tmpKeyNoB64[478 + i];
                }
                rsaP.DQ = arrPemExponent2;

                //Coefficient
                for (int i = 0; i < pemCoefficient; i++)
                {
                    arrPemCoefficient[i] = tmpKeyNoB64[545 + i];
                }
                rsaP.InverseQ = arrPemCoefficient;
            }
            else//公钥
            {
                for (int i = 0; i < pemModulus; i++)
                {
                    arrPemModulus[i] = tmpKeyNoB64[29 + i];
                }
                rsaP.Modulus = arrPemModulus;

                for (int i = 0; i < pemPublicExponent; i++)
                {
                    arrPemPublicExponent[i] = tmpKeyNoB64[159 + i];
                }
                rsaP.Exponent = arrPemPublicExponent;
            }

            return rsaP;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    provider.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~RSAHelper()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
