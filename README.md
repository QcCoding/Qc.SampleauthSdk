# SampleauthSdk

## Qc.SampleauthSdk

`Qc.SampleauthSdk` 是一个基于 `.NET Standard 2.0` 构建，对简单身份验证的封装。


### 使用 SampleauthSdk


#### 一.安装程序包

[![Nuget](https://img.shields.io/nuget/v/Qc.SampleauthSdk)](https://www.nuget.org/packages/Qc.SampleauthSdk/)

- dotnet cli  
  `dotnet add package Qc.SampleauthSdk`
- 包管理器  
  `Install-Package Install-Package Qc.SampleauthSdk`

#### 二.添加配置

> 如需实现自定义存储 AccessToken，动态获取应用配置，可自行实现接口 `ISampleauthSdkHook`  
> 默认提供 `DefaultSampleauthSdkHook`，存储 AccessToken 等信息到指定目录(默认./AppData)

```cs
using SampleauthSdk;
public void ConfigureServices(IServiceCollection services)
{
  //...
  services.AddSampleauthSdk<SampleauthSdk.DefaultSampleauthSdkHook>(opt =>
  {
      opt.ApiKey = "Api Key";
      opt.SecretKey = "Secret Key";
  });
  //...
}
```

#### 三.代码中使用

在需要地方注入`SampleauthService`后即可使用

### SampleauthConfig 配置项

Sampleauth文档地址: 

## 示例说明

`Qc.SampleauthSdk.Sample` 为示例项目，可进行测试
