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
    public partial class RSAProvider<T> : IDisposable
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
        public HashAlgorithm algorithm { get; set; }

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
        public string HashName => algorithm.HashSize switch
        {
            128 => "MD5",
            160 => "SHA1",
            256 => "SHA256",
            384 => "SHA384",
            512 => "SHA512",
            _ => throw new ArgumentOutOfRangeException()
        };

        /// <summary>
        /// 是否用OAEP填充
        /// </summary>
        public bool FullOAEP { get; set; }

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

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string Encrypt(byte[] bytes)
        {
            var value = provider.Encrypt(bytes, FullOAEP);
            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(string bytes)
        {
            return provider.Decrypt(Convert.FromBase64String(bytes), FullOAEP);
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
