using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TranslationApplicationCore.Services;
using StackExchange.Redis;

namespace TranslationApplicationCore
{
    public class Startup
    {

        const string CorsPolicyName = "CorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddLogging();

            services.AddCors(options=>
            {
                options.AddPolicy(CorsPolicyName,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(2520)));


            });

            services.AddMemoryCache();

            // services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<ITranslatorService, TranslatorService>();
            services.AddSingleton<IPageTranslatorService, PageTranslatorService>();

            // var cm = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
            //var cm = ConnectionMultiplexer.Connect("pratap.redis.cache.windows.net:6380,password=r0OeQeLO6WNohRRRtxZJdJqbqJ6iJe4ouAzCaMdTjRg=,ssl=True,abortConnect=False");

            var redisConnectionString = Configuration.GetSection("Redis")["ConnectionString"];
            var config = new ConfigurationOptions()
            {
                Ssl = true,
                KeepAlive = 0,
                AllowAdmin = true,
                EndPoints = { { "pratap.redis.cache.windows.net", 6380 } },
                ConnectTimeout = 5000,
                ConnectRetry = 5,
                SyncTimeout = 5000,
                Password= "r0OeQeLO6WNohRRRtxZJdJqbqJ6iJe4ouAzCaMdTjRg=",
                AbortOnConnectFail = false,
            };

           
            var cm = ConnectionMultiplexer.Connect(config);
           
            services.AddSingleton<IConnectionMultiplexer>(cm);


           

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Translation Micro Service", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.

            });


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
                //options.InstanceName = "localRedis_";
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors(CorsPolicyName);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Translation Micro Service");
            });
        }
    }
}
