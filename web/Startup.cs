using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeanCloud.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace web
{
    public struct RedisConfigurationOptions
    {
        public string Password { get; set; }
        public KeyValuePair<string, UInt16> EndPoint { get; set; }
    }

    public class Startup
    {
        public string GetLeanCacheRedisConnectionString(string instanceName)
        {
            return Environment.GetEnvironmentVariable($"REDIS_URL_{instanceName}");
        }

        public RedisConfigurationOptions GetRedisConfiguration(string instanceName)
        {
            var connectionString = GetLeanCacheRedisConnectionString(instanceName);
            var disassembleStrArray = connectionString.Split(':');
            var passwordAndEndPointName = disassembleStrArray[2].Split('@');
            var password = passwordAndEndPointName[0];
            var endPointName = passwordAndEndPointName[1];
            var port = UInt16.Parse(disassembleStrArray[3]);
            return new RedisConfigurationOptions()
            {
                EndPoint = new KeyValuePair<string, ushort>(endPointName, port),
                Password = password
            };
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var instanceName = "dev";
            services.UseLeanCache(instanceName);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCloud();
            app.UseLog();
            app.UseHttpsRedirect();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
