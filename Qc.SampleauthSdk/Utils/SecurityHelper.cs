using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Qc.SampleauthSdk.Utils
{
    public class SecurityHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string ComputePasswordHash(string password)
        {
            var data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = new StringBuilder();

            foreach (var b in data)
            {
                hash.Append(b.ToString("x2"));
            }

            return hash.ToString().ToUpper();
        }
        private const string TOKEN_KEY = "___Sklasdf#l09%@sd5lasdf";
        /// <summary>
        /// 生成登录的Token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userkey"></param>
        /// <returns></returns>
        public static string GetSignToken(string username, string userkey)
        {
            return ComputePasswordHash(username + userkey + TOKEN_KEY);
        }
    }
}
