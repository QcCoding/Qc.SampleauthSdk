using System;
using System.Collections.Generic;
using System.Text;

namespace Qc.SampleauthSdk
{
    /// <summary>
    /// 用户配置
    /// </summary>
    public class SampleauthUserItem
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string Userpwd { get; set; }
        /// <summary>
        /// 加密密钥
        /// </summary>
        public string Userkey { get; set; }
    }
}
