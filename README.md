## Qc.SampleauthSdk

`Qc.SampleauthSdk` 是一个基于 `.NET Standard 2.0` 构建的超简单登录中间件，方便的给 swagger,一些私有资源添加拦截登录页，还可以根据需要配置不同语言


### 使用 SampleauthSdk


#### 一.安装程序包

[![Nuget](https://img.shields.io/nuget/v/Qc.SampleauthSdk)](https://www.nuget.org/packages/Qc.SampleauthSdk/)

- dotnet cli  
  `dotnet add package Qc.SampleauthSdk`
- 包管理器  
  `Install-Package Install-Package Qc.SampleauthSdk`

#### 二.添加配置

```cs
using Qc.SampleauthSdk;

public void ConfigureServices(IServiceCollection services)
{
    // 方式1. 可使用配置文件配置，app.UseSampleauthSdk(opt=>{//此处配置的选项优先级更大});
    // services.AddSampleauthSdk(Configuration.GetSection("SampleauthSetting").Bind);
}
public void Configure(IApplicationBuilder app)
{
	// 方式1. 若已通过services.AddSampleauthSdk(opt)配置，则此处无需配置
    //app.UseSampleauthSdk()
	// 方式2. 直接再此处进行配置，可通过分隔符【,|】匹配多个前缀
	app.UseSampleauthSdk(opt =>
	{
		//默认不缺分大小写
		opt.PathIsConvertToLower = true;
		opt.RoutePrefix = "swagger";
		opt.DisabledAutoRedirectLogin = false;
		opt.SampleauthList = new List<SampleauthUserItem>()
		{
				new SampleauthUserItem()
				{
					Username="swagger",
					Userpwd="123456"
				}
		};
		opt.SignInBeforeHook = (httpcontext, username, userpwd) =>
		{
			Console.WriteLine("触发登录前钩子");
			Console.WriteLine("用户名：" + username);
			return false;
		};
		opt.SignCheckBeforeHook = (httpContext) =>
		{
			Console.WriteLine("触发权限校验前钩子");
			return false;
		};
		opt.SignOutBeforeHook = (httpContext) =>
		{
			Console.WriteLine("触发退出钩子");
			return false;
		};

	});
}
```

### SampleauthConfig 配置项

```cs

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
```


## 示例说明

`Qc.SampleauthSdk.Sample` 为示例项目，可进行测试
