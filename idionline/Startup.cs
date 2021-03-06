﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Mongo;
using Idionline.Models;
using Microsoft.Extensions.Hosting;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

namespace Idionline
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            };
            var storageOptions = new MongoStorageOptions
            {
                MigrationOptions = migrationOptions
            };
            services.Configure<IdionlineSettings>(Configuration.GetSection("IdionlineSettings"));
            services.AddHangfire(options => options.UseMongoStorage("mongodb://localhost/IdionlineDB", storageOptions));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddHttpClient();
            services.AddTransient<DataAccess>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            app.UseHttpsRedirection();
            app.UseHangfireDashboard();
            BackgroundJobServerOptions options = new BackgroundJobServerOptions { WorkerCount = 1 };
            app.UseHangfireServer(options);
            //生成每日成语。
            RecurringJob.AddOrUpdate<DataAccess>(x => x.GenLI(), Cron.Daily, TimeZoneInfo.Local);
        }
    }
}
