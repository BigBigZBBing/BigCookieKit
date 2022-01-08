using BigCookieKit.IO;
using BigCookieKit.XML;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BigCookieKit.Algorithm
{
    /// <summary>
    /// Asn.1规范:T-REC-X.690-201508
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        public RSAParameters publickKey { get; set; }

        /// <summary>
        /// 私钥参数
        /// </summary>
        public RSAParameters privateKey { get; set; }

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
            var modulus = GetNodeBase64Byte(packet, "Modulus");
            var exponent = GetNodeBase64Byte(packet, "Exponent");
            int halfN = (modulus.Length + 1) / 2;
            paramter.Modulus = modulus;
            paramter.Exponent = exponent;
            paramter.D = GetNodeByte(packet, "D", halfN);
            paramter.DP = GetNodeByte(packet, "DP", halfN);
            paramter.DQ = GetNodeByte(packet, "DQ", halfN);
            paramter.InverseQ = GetNodeByte(packet, "InverseQ", halfN);
            paramter.P = GetNodeByte(packet, "P", halfN);
            paramter.Q = GetNodeByte(packet, "Q", halfN);
            provider.ImportParameters(paramter);
        }

        /// <summary>
        /// 导入RSA的base64密钥
        /// </summary>
        /// <param name="secret"></param>
        public void ImportBase64(string publicSecret, string privateSecret, bool isPKCS8 = false, string password = null)
        {
            var pub = Convert.FromBase64String(publicSecret);
            var prv = Convert.FromBase64String(privateSecret);
            provider.ImportRSAPublicKey(pub, out var _read);
            if (isPKCS8)
            {
                if (string.IsNullOrEmpty(password))
                {
                    provider.ImportPkcs8PrivateKey(prv, out _read);
                }
                else
                {
                    provider.ImportEncryptedPkcs8PrivateKey(Convert.FromBase64String(password), prv, out _read);
                }
            }
            else
            {
                provider.ImportRSAPrivateKey(prv, out _read);
            }
        }

        /// <summary>
        /// 导入RSA的pem密钥
        /// </summary>
        /// <param name="pem"></param>
        public void ImportPem(string publicPem, string privatePem, bool isPKCS8 = false, string password = null)
        {
            publicPem = publicPem.Replace("\n", "");
            publicPem = publicPem.Replace("\r", "");
            publicPem = publicPem.Replace("-----BEGIN PUBLIC KEY-----", "");
            publicPem = publicPem.Replace("-----END PUBLIC KEY-----", "");
            privatePem = privatePem.Replace("\n", "");
            privatePem = privatePem.Replace("\r", "");
            privatePem = privatePem.Replace("-----BEGIN RSA PRIVATE KEY-----", "");
            privatePem = privatePem.Replace("-----END RSA PRIVATE KEY-----", "");
            string publicSecret = publicPem;
            string privateSecret = privatePem;

            ImportBase64(publicSecret, privateSecret, isPKCS8, password);
        }

        /// <summary>
        /// 获取XML公钥密钥
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
        /// 获取Base64公钥密钥
        /// </summary>
        /// <returns>
        /// <para>Item1:PublicKey</para>
        /// <para>Item2:PrivateKey</para>
        /// </returns>
        public Tuple<string, string> GetBase64Secret(bool isPKCS8 = false, string password = null)
        {
            return Tuple.Create(
                Convert.ToBase64String(ExportPublicKey()),
                Convert.ToBase64String(ExportPrivateKey()));
        }

        /// <summary>
        /// 获取Pem公钥密钥
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string> GetPemSecret()
        {
            return Tuple.Create(ExportPublicPem(), ExportPrivatePem());
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

        #region Base

        private byte[] ExportPublicKey()
        {
            return provider.ExportRSAPublicKey();
        }

        private byte[] ExportPrivateKey(bool isPKCS8 = false, string password = null)
        {
            return isPKCS8 ? string.IsNullOrEmpty(password) ?
                          provider.ExportPkcs8PrivateKey() :
                          provider.ExportEncryptedPkcs8PrivateKey(password.AsSpan(), new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, new HashAlgorithmName(HashName), 0))
                          : provider.ExportRSAPrivateKey();
        }

        private string ExportPrivatePem()
        {
            StringWriter outputStream = new StringWriter();

            var bytes = ExportPrivateKey();

            var base64 = Convert.ToBase64String(bytes).ToCharArray();
            outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
            // Output as Base64 with lines chopped at 64 characters
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                outputStream.Write("\n");
            }
            outputStream.Write("-----END RSA PRIVATE KEY-----");

            return outputStream.ToString();
        }

        private string ExportPublicPem()
        {
            StringWriter outputStream = new StringWriter();

            var bytes = ExportPublicKey();

            var base64 = Convert.ToBase64String(bytes).ToCharArray();
            // WriteLine terminates with \r\n, we want only \n
            outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                outputStream.Write("\n");
            }
            outputStream.Write("-----END PUBLIC KEY-----");

            return outputStream.ToString();
        }

        #endregion

        #region 解析T-REC-X.690-201508

        private byte[] ExportPublicKeyTest()
        {
            List<byte> bytes = new List<byte>();
            WriteTag(bytes, 16); //Sequence
            WriteLength(bytes, -1);
            WriteTagAndIntegerUnsigned(bytes, publickKey.Modulus);
            WriteTagAndIntegerUnsigned(bytes, publickKey.Exponent);
            PopSequence(bytes);
            return bytes.ToArray();
        }

        private byte[] ExportPrivateKeyTest(bool isPKCS8 = false, string password = null)
        {
            List<byte> bytes = new List<byte>();
            WriteTag(bytes, 16); //Sequence
            WriteLength(bytes, -1);
            WriteTagAndIntegerUnsigned(bytes, 0);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Modulus);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Exponent);
            WriteTagAndIntegerUnsigned(bytes, privateKey.D);
            WriteTagAndIntegerUnsigned(bytes, privateKey.P);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Q);
            WriteTagAndIntegerUnsigned(bytes, privateKey.DP);
            WriteTagAndIntegerUnsigned(bytes, privateKey.DQ);
            WriteTagAndIntegerUnsigned(bytes, privateKey.InverseQ);
            PopSequence(bytes);
            return bytes.ToArray();
        }

        private void PopSequence(List<byte> bytes)
        {
            int containedLength = bytes.Count - 2;
            var shiftSize = GetEncodedLengthSubsequentByteCount(containedLength);
            for (int i = 0; i < shiftSize; i++) bytes.Add(0);
            byte[] dest = bytes.ToArray();
            Buffer.BlockCopy(dest, 2, dest, 2 + shiftSize, containedLength);
            bytes.Clear();
            bytes.AddRange(dest);
            WriteOffsetLength(bytes, containedLength, 1);
        }

        private void WriteTagAndIntegerUnsigned(List<byte> bytes, params byte[] value)
        {
            // Integer标识
            bytes.Add(2);
            // 写入长度
            if (value[0] >= 0x80)
            {
                WriteLength(bytes, value.Length + 1);
                bytes.Add(0);
            }
            else
            {
                WriteLength(bytes, value.Length);
            }
            // 写入内容
            bytes.AddRange(value);
        }

        private void WriteLength(List<byte> bytes, int length)
        {
            if (length == -1)
            {
                bytes.Add(0X80);
                return;
            }

            if (length >= 0x80)
            {
                var lengthLength = GetEncodedLengthSubsequentByteCount(length);
                bytes.Add((byte)(0x80 | lengthLength));
                var remaining = length;
                do
                {
                    bytes.Add((byte)remaining);
                    remaining >>= 8;
                } while (remaining > 0);
            }
            else bytes.Add((byte)length);
        }

        private void WriteOffsetLength(List<byte> bytes, int length, int offset)
        {
            if (length >= 0x80)
            {
                var lengthLength = GetEncodedLengthSubsequentByteCount(length);
                int idx = offset + lengthLength;
                bytes[offset] = (byte)(0x80 | lengthLength);
                var remaining = length;
                do
                {
                    bytes[idx] = (byte)remaining;
                    remaining >>= 8;
                    idx--;
                } while (remaining > 0);
            }
            else bytes.Add((byte)length);
        }

        private void WriteTag(List<byte> bytes, int tagValue)
        {
            var spaceRequired = BarEncodeSize(tagValue);

            if (spaceRequired == 1)
            {
                bytes.Add((byte)(32 | tagValue));
                return;
            }
            else
            {
                bytes.Add((byte)(32 | tagValue));

                int count = bytes.Count;
                int remaining = tagValue;

                while (remaining > 0)
                {
                    int segment = remaining & 0x7F; // 超过127则是0

                    if (count != bytes.Count)
                    {
                        bytes[--count] = (byte)segment;
                    }
                    else
                    {
                        bytes.Add((byte)segment);
                    }
                    remaining >>= 7; // 推7位:大于127才会大于0
                }
            }
        }

        private int BarEncodeSize(int tagValue)
        {
            if (tagValue < 31)
            {
                return 1;
            }
            else return 2;
        }

        private int GetEncodedLengthSubsequentByteCount(int value_len)
        {
            if (value_len < 0)
                throw new OverflowException();
            if (value_len <= 0x7F)
                return 0;
            if (value_len <= byte.MaxValue)
                return 1;
            if (value_len <= ushort.MaxValue)
                return 2;
            if (value_len <= 0x00FFFFFF)
                return 3;

            return 4;
        }

        #endregion

        #region 解析XML算法

        /// <summary>
        /// 根据Xml包获取子节点的字节流
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private byte[] GetNodeByte(XmlPacket packet, string node, int sizeHint)
        {
            var _node = packet.Node.FirstOrDefault(x => x.Info.Name == node);
            if (_node != null && !string.IsNullOrEmpty(_node.Info.Text))
            {
                byte[] ret = new byte[sizeHint];
                if (Convert.TryFromBase64Chars(_node.Info.Text.AsSpan(), ret, out int written))
                {
                    if (written == sizeHint)
                    {
                        return ret;
                    }

                    int shift = sizeHint - written;
                    Buffer.BlockCopy(ret, 0, ret, shift, written);
                    ret.AsSpan(0, shift).Clear();
                    return ret;
                }
                return Convert.FromBase64String(_node.Info.Text);
            }
            return null;
        }

        /// <summary>
        /// 根据Xml包获取子节点的内容
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private byte[] GetNodeBase64Byte(XmlPacket packet, string node)
        {
            var _node = packet.Node.FirstOrDefault(x => x.Info.Name == node);
            if (_node != null && !string.IsNullOrEmpty(_node.Info.Text))
            {
                return Convert.FromBase64String(_node.Info.Text);
            }
            return null;
        }

        #endregion
    }

}
