﻿using System.IO;
using Akka.Configuration;
using AkkaExchange.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
                    .AddAkkaExchangeDependencies()
                    .Build();

                var configString = File.ReadAllText("config.txt");
                var config = ConfigurationFactory.ParseString(configString);

                return new AkkaExchange(container, config);
            });
            services.AddTransient<HubSubscriptionCollection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
