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
            provider.FromXmlString(xml);
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
