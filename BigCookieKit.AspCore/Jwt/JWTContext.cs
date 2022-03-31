using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BigCookieKit.AspCore.Jwt
{
    public class JWTContext
    {
        /// <summary>
        /// 秘钥，可以从配置文件中获取
        /// </summary>
        public static string SecurityKey { get; set; }

        /// <summary>
        /// 创建jwttoken,源码自定义
        /// <code>
        /// sub 主题
        /// jti 唯一编号
        /// iss 签发人
        /// aud 接收人
        /// iat 签发时间 *
        /// nbf 生效时间 *
        /// exp 过期时间 *
        /// </code>
        /// </summary>
        /// <param name="payLoad"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string CreateToken(Dictionary<string, object> payLoad, int expiresMinute, Dictionary<string, object> header = null)
        {
            if (header == null)
            {
                header = new Dictionary<string, object>(new List<KeyValuePair<string, object>>() {
                    new KeyValuePair<string, object>("alg", "HS256"),
                    new KeyValuePair<string, object>("typ", "JWT")
                });
            }

            //添加jwt可用时间（应该必须要的）
            var now = DateTime.UtcNow;

            if (payLoad.ContainsKey("iat")) payLoad["iat"] = ToUnixEpochDate(now);
            else payLoad.Add("iat", ToUnixEpochDate(now));

            if (payLoad.ContainsKey("nbf")) payLoad["nbf"] = ToUnixEpochDate(now);
            else payLoad.Add("nbf", ToUnixEpochDate(now));

            if (payLoad.ContainsKey("exp")) payLoad["exp"] = ToUnixEpochDate(now.Add(TimeSpan.FromMinutes(expiresMinute)));
            else payLoad.Add("exp", ToUnixEpochDate(now.Add(TimeSpan.FromMinutes(expiresMinute))));

            var encodedHeader = Base64UrlEncode(JsonConvert.SerializeObject(header));
            var encodedPayload = Base64UrlEncode(JsonConvert.SerializeObject(payLoad));

            var helper = new DESHelper();
            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(SecurityKey));
            var encodedSignature = helper.Encrypt(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(encodedHeader, ".", encodedPayload))));

            var encodedJwt = string.Concat(encodedHeader, ".", encodedPayload, ".", encodedSignature);
            return encodedJwt;
        }

        /// <summary>
        /// 验证身份 验证签名的有效性,
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad">自定义各类验证； 是否包含那种申明，或者申明的值， </param>
        /// 例如：payLoad["aud"]?.ToString() == "roberAuddience";
        /// 例如：验证是否过期 等
        /// <returns></returns>
        public static bool Validate(string encodeJwt, Func<Dictionary<string, object>, bool> validatePayLoad)
        {
            var helper = new DESHelper();
            var success = true;
            var jwtArr = encodeJwt.Split('.');
            var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlDecode(jwtArr[0]));
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlDecode(jwtArr[1]));

            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(SecurityKey));
            //首先验证签名是否正确（必须的）
            success = success && string.Equals(jwtArr[2], helper.Encrypt(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1])))));
            if (!success)
            {
                return success;//签名不正确直接返回
            }
            //其次验证是否在有效期内（也应该必须）
            var now = ToUnixEpochDate(DateTime.UtcNow);
            success = success && (now >= long.Parse(payLoad["nbf"].ToString()) && now < long.Parse(payLoad["exp"].ToString()));

            //再其次 进行自定义的验证
            success = success && validatePayLoad(payLoad);

            return success;
        }

        /// <summary>
        /// 获取jwt中的payLoad
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetPayLoad(string encodeJwt)
        {
            var helper = new DESHelper();
            var jwtArr = encodeJwt.Split('.');
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(helper.Decrypt(jwtArr[1]));
            return payLoad;
        }

        public static long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public static string Base64UrlEncode(string content) => Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

        public static string Base64UrlDecode(string sign) => Encoding.UTF8.GetString(Convert.FromBase64String(sign));
    }
}
