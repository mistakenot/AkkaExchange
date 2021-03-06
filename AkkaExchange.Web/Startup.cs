﻿using System;
using System.IO;
using Akka.Configuration;
using AkkaExchange.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AkkaExchange.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton(sp =>
            {
                var container = new Autofac.ContainerBuilder()
                    .AddAkkaExchangeDependencies();

                var configString = File.ReadAllText("config.txt");
                var config = ConfigurationFactory.ParseString(configString);

                return new AkkaExchange(container, config, sp.GetService(typeof(ILogger<AkkaExchange>)) as ILogger<AkkaExchange>);
            });

            services.AddTransient<HubSubscriptionCollection>();
            services.AddTransient<HubClientCollection>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<AkkaExchangeHub>("hub");
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
