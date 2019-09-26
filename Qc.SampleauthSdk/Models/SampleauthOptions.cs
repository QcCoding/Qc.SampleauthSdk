using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qc.SampleauthSdk
{
    public class SampleauthOptions
    {
        /// <summary>
        /// 路由前缀
        /// </summary>
        public string RoutePrefix { get; set; }
        /// <summary>
        /// 禁用自动跳转到登录
        /// </summary>
        public bool DisabledAutoRedirectLogin { get; set; }
        /// <summary>
        /// 路径全部转换为小写
        /// </summary>
        public bool PathIsConvertToLower { get; set; }
        /// <summary>
        /// 登录路径
        /// </summary>
        public string LoginPath { get; set; } = "login";
        /// <summary>
        /// 退出路径，访问/{RoutePrefix}/{LogoutPath}即可退出
        /// </summary>
        public string LogoutPath { get; set; } = "logout";
        /// <summary>
        /// 账号密码列表
        /// </summary>
        public List<SampleauthUserItem> SampleauthList { get; set; }
        /// <summary>
        /// 页面文案配置
        /// </summary>
        public Dictionary<string, string> PageSetting { get; set; }
        /// <summary>
        /// 获取页面配置
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, string> GetPageConfig()
        {
            var dic = new Dictionary<string, string>()
            {
                {SampleauthPageConst.LoginTextErrorMsg,"登录失败, 账号或密码错误!" },
                {SampleauthPageConst.LoginTextPageTitle,"系统登录" },
                {SampleauthPageConst.LoginPlaceholderUsername,"请输入用户名" },
                {SampleauthPageConst.LoginPlaceholderUserpwd,"请输入密码" },
                {SampleauthPageConst.LoginTextSubmitButton,"登  录" },
                {SampleauthPageConst.LoginHeadStyle,string.Empty },
                {SampleauthPageConst.LoginBodyScript,string.Empty },
            };
            if (PageSetting?.Count > 0)
            {
                foreach (var item in PageSetting)
                {
                    if (dic.ContainsKey(item.Key))
                    {
                        dic[item.Key] = item.Value;
                    }
                }
            }
            return dic;
        }
        /// <summary>
        /// 登录前的处理程序钩子
        /// </summary>
        /// <returns></returns>
        public Func<HttpContext, string, string, bool> SignInBeforeHook = (HttpContext httpContext, string username, string userpwd) => false;
        /// <summary>
        /// 登录校验钩子
        /// </summary>
        /// <returns></returns>
        public Func<HttpContext, bool> SignCheckBeforeHook = (HttpContext httpContext) => false;
        /// <summary>
        /// 登录前的处理程序钩子
        /// </summary>
        /// <returns></returns>
        public Func<HttpContext, bool> SignOutBeforeHook = (HttpContext httpContext) => false;
    }

}
