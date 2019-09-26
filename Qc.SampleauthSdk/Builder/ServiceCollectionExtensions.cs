using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qc.SampleauthSdk
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Sampleauth SDK ，配置用户名密码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddSampleauthSdk(this IServiceCollection services, Action<SampleauthOptions> optionsAction)
        {
            if (optionsAction != null)
            {
                services.Configure(optionsAction);
            }

            return services;
        }
    }
}
