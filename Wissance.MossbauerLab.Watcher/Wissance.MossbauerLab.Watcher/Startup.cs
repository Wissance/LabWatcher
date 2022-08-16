using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.MossbauerLab.Watcher.Web.Extensions;
using Wissance.MossbauerLab.Watcher.Web.Jobs;
using Wissance.MossbauerLab.Watcher.Web.Store;

namespace Wissance.MossbauerLab.Watcher.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
            _config = Configuration.GetSection(ApplicationConfigSectionName).Get<ApplicationConfig>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            ConfigureLogging(services);
            ConfigureAppServices(services);
            ConfigureWebApi(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.ConfigureSqliteDbContext<ModelContext>(_config.ConnStr);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ModelContext modelContext = serviceProvider.GetRequiredService<ModelContext>();
            modelContext.Database.Migrate();
        }


        private void ConfigureLogging(IServiceCollection services)
        {
            Log.Logger = new Serilog.LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(Configuration).AddConsole());
            services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(Configuration).AddDebug());
            services.AddLogging(loggingBuilder => loggingBuilder.AddConfiguration(Configuration).AddSerilog(dispose: true));
        }

        private void ConfigureAppServices(IServiceCollection services)
        {
            ConfigureSmb(services);
            ConfigureRegularJobs(services);
        }

        private void ConfigureWebApi(IServiceCollection services)
        {

        }

        private void ConfigureSmb(IServiceCollection services)
        {
            services.AddScoped<IFileStoreService>(x =>
            {
                return new NetworkFileSystemStoreService(_config.Sm2201SpectraStoreSettings, x.GetRequiredService<ILoggerFactory>());
            });
        }

        private void ConfigureRegularJobs(IServiceCollection services)
        {
            services.AddQuartz(quartz =>
            {
                quartz.UseMicrosoftDependencyInjectionJobFactory();

                quartz.AddJob<SpectraIndexerJob>(job => job.WithIdentity(nameof(SpectraIndexerJob)));
                //todo: umv: move in config (every 3 hours)
                quartz.AddTrigger(trigger => trigger.ForJob(nameof(SpectraIndexerJob))
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(60)));
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        private const string ApplicationConfigSectionName = "Application";

        private readonly ApplicationConfig _config;
    }
}
