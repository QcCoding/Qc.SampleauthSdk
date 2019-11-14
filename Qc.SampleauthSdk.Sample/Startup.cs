
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qc.SampleauthSdk;
using Swashbuckle.AspNetCore.Swagger;

namespace Qc.SampleauthSdk.Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSampleauthSdk(Configuration.GetSection("SampleauthSetting").Bind);
            //
            //�˴��������ȼ���С
            //            services.AddSampleauthSdk(opt =>
            //            {
            //                //Ĭ�ϲ�ȱ�ִ�Сд
            //                //opt.RoutePrefix = "swagger";
            //                opt.PageSetting = new Dictionary<string, string>()
            //                {
            //                                {SampleauthPageConst.LoginTextPageTitle,"������Ŀ-��¼" },
            //                                {SampleauthPageConst.LoginHeadStyle,@"<style>
            //            body{background-color: #0c2725;}
            //            .login_input{1px solid #0c2725;}</style>
            //            " },
            //                                {SampleauthPageConst.LoginBodyScript,@"<script>
            //            console.log('hello world')
            //</script>
            //            " }
            //                };
            //                opt.SampleauthList = new List<SampleauthUserItem>()
            //                {
            //                                 new SampleauthUserItem()
            //                                 {
            //                                      Username="yimo",
            //                                      Userpwd="123456"
            //                                 }
            //                };
            //            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // �˴��������ȼ��ϴ�
            app.UseSampleauthSdk(opt =>
            {
                //ֻ����һ����Ч��Ҫʵ��ÿ����¼ҳ��һ������ʹ��opt.RenderPageHook
                Configuration.GetSection("SampleauthSetting").Bind(opt);
                //Ĭ�ϲ�ȱ�ִ�Сд
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
                    Console.WriteLine("������¼ǰ����");
                    Console.WriteLine("�û�����" + username);
                    return false;
                };
                opt.SignCheckBeforeHook = (httpContext) =>
                {
                    Console.WriteLine("����Ȩ��У��ǰ����");
                    return false;
                };
                opt.SignOutBeforeHook = (httpContext) =>
                {
                    Console.WriteLine("�����˳�����");
                    return false;
                };
                opt.RenderPageHook = (httpContext, pageContent) =>
                {
                    pageContent += "<script>console.log('����׷������')</script>";
                    pageContent = pageContent.Replace("required", "");
                    return pageContent;
                };
            });
            app.UseSampleauthSdk(opt =>
            {
                Configuration.GetSection("SampleauthSetting").Bind(opt);
                opt.RoutePrefix = "test";
                opt.SampleauthList = new List<SampleauthUserItem>()
                {
                     new SampleauthUserItem()
                     {
                          Username="test",
                          Userpwd="123456"
                     }
                };
            });
            app.UseSampleauthSdk(opt =>
            {
                Configuration.GetSection("SampleauthSetting").Bind(opt);
                opt.RoutePrefix = "test233,test333";
                opt.SampleauthList = new List<SampleauthUserItem>()
                {
                     new SampleauthUserItem()
                     {
                          Username="test233",
                          Userpwd="123456"
                     }
                };
            });

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseStaticFiles();
        }
    }
}
