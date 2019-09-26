using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace Qc.SampleauthSdk
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseSampleauthSdk(this IApplicationBuilder app, Action<SampleauthOptions> setupAction = null)
        {
            if (setupAction == null)
            {
                app.UseMiddleware<SampleauthMiddleware>();
            }
            else
            {
                SampleauthOptions authOptions = new SampleauthOptions();
                setupAction(authOptions);
                UseMiddlewareExtensions.UseMiddleware<SampleauthMiddleware>(app, new object[1]
                {
                    authOptions
                });
            }
            return app;
        }
    }
}
