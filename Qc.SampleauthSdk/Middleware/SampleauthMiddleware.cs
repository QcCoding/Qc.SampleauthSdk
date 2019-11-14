using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qc.SampleauthSdk.Utils;

namespace Qc.SampleauthSdk
{
    public class SampleauthMiddleware
    {
        private readonly RequestDelegate _next;
        private SampleauthOptions _options;
        public SampleauthMiddleware() { }
        public SampleauthMiddleware(RequestDelegate next, SampleauthOptions options = null)
        {
            _next = next;
            _options = options;
        }
        private const string SAMPLE_ATUH_SDK_COOKIE = nameof(SAMPLE_ATUH_SDK_COOKIE);
        private const string LOGIN_FILE_NAME = "login.html";

        public async Task Invoke(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetService<IOptions<SampleauthOptions>>()?.Value;
            var _sampleauthList = _options?.SampleauthList;
            if (_sampleauthList?.Count == 0)
            {
                Console.WriteLine("SampleauthSdk no config users!!!");
                //未配置用户则不启用
                await _next(context);
                return;
            }
            var _method = context.Request.Method.ToLower();
            var _path = context.Request.PathBase.Value + context.Request.Path.Value;
            var _routePrefix = _options.RoutePrefix ?? string.Empty;
            var _loginPath = _options.LoginPath;
            var _logoutPath = _options.LogoutPath;
            if (_options.PathIsConvertToLower)
            {
                _routePrefix = _routePrefix.ToLower();
                _path = _path.ToLower();
                _loginPath = _loginPath.ToLower();
                _logoutPath = _logoutPath.ToLower();
            }
            if (_routePrefix.Contains(",") || _routePrefix.Contains("|"))
            {
                var morePrefixs = _routePrefix.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => "/" + s);
                //为空
                _routePrefix = morePrefixs.FirstOrDefault(s => (_path + "/").StartsWith(s + "/"));
            }
            if (!string.IsNullOrEmpty(_routePrefix) && !_routePrefix.StartsWith("/"))
            {
                _routePrefix = "/" + _routePrefix;
            }
            if (_routePrefix == null || !(_path + "/").StartsWith(_routePrefix + "/"))
            {
                await _next(context);
                return;
            }
            else if (_path.StartsWith($"{_routePrefix}/{_loginPath}"))
            {
                //登录
                if (_method == "get")
                {
                    await WriteStaticPage(context, LOGIN_FILE_NAME, _options.GetPageConfig());
                    return;
                }
                else if (_method == "post")
                {
                    string username = context.Request.Form["username"].ToString().Trim();
                    string userpwd = context.Request.Form["userpwd"].ToString().Trim();
                    // 用户登录处理
                    if (_options.SignInBeforeHook(context, username, userpwd))
                    {
                        return;
                    }

                    var existUser = _sampleauthList?.FirstOrDefault(s => s.Username == username && s.Userpwd == userpwd);
                    // 用户是否存在
                    if (existUser == null)
                    {
                        context.Response.Redirect(GetRedirectPath(context, new Dictionary<string, string>() { { "msg", _options.GetPageConfig()[SampleauthPageConst.LoginTextErrorMsg] } }));
                        return;
                    }
                    string userkey = existUser.Userkey;
                    context.Response.Cookies.Append(SAMPLE_ATUH_SDK_COOKIE, Utils.SecurityHelper.GetSignToken(username, userkey));

                    string returnUrl = context.Request.Query["returnUrl"];
                    returnUrl = string.IsNullOrEmpty(returnUrl) ? $"{_routePrefix}/" : returnUrl;
                    context.Response.Redirect(returnUrl);
                    return;
                }
            }
            else if (_path.StartsWith($"{_routePrefix}/{_logoutPath}"))
            {
                // 用户校验处理
                if (_options.SignOutBeforeHook(context))
                {
                    return;
                }
                //退出
                context.Response.Cookies.Delete(SAMPLE_ATUH_SDK_COOKIE);
                context.Response.Redirect($"{_routePrefix}/{_loginPath}");
                return;
            }
            else
            {
                // 用户校验处理
                if (_options.SignCheckBeforeHook(context))
                {
                    await _next(context);
                    return;
                }

                //身份验证
                var encryptStr = context.Request.Cookies[SAMPLE_ATUH_SDK_COOKIE];
                if (!string.IsNullOrEmpty(encryptStr) && _sampleauthList?.Any(s => SecurityHelper.GetSignToken(s.Username, s.Userkey) == encryptStr) == true)
                {
                    await _next(context);
                    return;
                }
                if (_options.DisabledAutoRedirectLogin)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                context.Response.Redirect($"{_routePrefix}/{_loginPath}?returnUrl=" + System.Web.HttpUtility.UrlEncode(GetRedirectPath(context)));
                return;
            }
        }
        /// <summary>
        /// 缓存的页面Html
        /// </summary>

        public static Dictionary<string, string> CachePageHtml = new Dictionary<string, string>();
        /// <summary>
        /// 使用登录页
        /// </summary>
        /// <returns></returns>
        private async Task WriteStaticPage(HttpContext context, string page, Dictionary<string, string> pageConfig)
        {
            context.Response.ContentType = "text/html;charset=utf-8";
            context.Response.StatusCode = StatusCodes.Status200OK;
            if (CachePageHtml.ContainsKey(page))
            {
                await context.Response.WriteAsync(CachePageHtml[page]);
            }
            else
            {
                var currentAssembly = typeof(SampleauthOptions).GetTypeInfo().Assembly;
                var resourceStream = currentAssembly.GetManifestResourceStream($"{currentAssembly.GetName().Name}.{page}");
                string pageContent = new StreamReader(resourceStream).ReadToEnd();
                foreach (var item in pageConfig)
                {
                    if (item.Key.StartsWith("<tmp-"))
                    {
                        pageContent = pageContent.Replace(item.Key, item.Value);
                    }
                    else
                    {
                        pageContent = pageContent.Replace("{{" + item.Key + "}}", item.Value);
                    }
                }
                pageContent = _options.RenderPageHook(context, pageContent);
                // 缓存页面
                if (!CachePageHtml.ContainsKey(page))
                {
                    CachePageHtml.Add(page, pageContent);
                }
                await context.Response.WriteAsync(pageContent, Encoding.UTF8);
            }
        }
        /// <summary>
        /// 从当前路径中获取添加参数后的Url
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="queryDic"></param>
        /// <returns></returns>
        private string GetRedirectPath(HttpContext httpContext, Dictionary<string, string> queryDic = null)
        {
            var httpQuery = httpContext.Request.Query.Where(s => queryDic == null || !queryDic.ContainsKey(s.Key)).ToDictionary(s => s.Key, s => s.Value.ToString());
            queryDic = queryDic ?? new Dictionary<string, string>();
            var queryString = new QueryString();
            foreach (var item in queryDic)
            {
                queryString = queryString.Add(item.Key, item.Value);
            }
            foreach (var item in httpQuery)
            {
                queryString = queryString.Add(item.Key, item.Value);
            }

            var builder = new StringBuilder()
                .Append(httpContext.Request.PathBase)
                .Append(httpContext.Request.Path)
                .Append(queryString);
            return builder.ToString();
        }
    }
}
