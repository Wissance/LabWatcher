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
using Wissance.MossbauerLab.Watcher.Common;
using Wissance.MossbauerLab.Watcher.Common.Data.Notification;
using Wissance.MossbauerLab.Watcher.Data;
using Wissance.MossbauerLab.Watcher.Common.Extensions;
using Wissance.MossbauerLab.Watcher.Services.Notification;
using Wissance.MossbauerLab.Watcher.Services.Store;
using Wissance.MossbauerLab.Watcher.Web.Config;
using Wissance.MossbauerLab.Watcher.Web.Services.Jobs;
using Wissance.MossbauerLab.Watcher.Web.Services.Store;

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
            // config 
            services.AddSingleton<ApplicationConfig>(x => Configuration.GetSection(ApplicationConfigSectionName).Get<ApplicationConfig>());
            // Access to shared folder
            ConfigureSharedFolderAccess(services);
            // regular jobs (watch)
            ConfigureRegularJobs(services);
            // notifications
            ConfigureNotificationServices(services);
        }

        private void ConfigureWebApi(IServiceCollection services)
        {

        }

        private void ConfigureNotificationServices(IServiceCollection services)
        {
            services.AddScoped<EmailNotifier>(x =>
            {
                return new EmailNotifier(_config.NotificationSettings.MailSettings, x.GetRequiredService<ILoggerFactory>());
            });
            services.AddScoped<TelegramNotifier>(x =>
            {
                return new TelegramNotifier(_config.NotificationSettings.TelegramSettings, 
                    new Dictionary<SpectrometerEvent, MessageTemplate>()
                    {
                        { SpectrometerEvent.SpectrumSaved, new MessageTemplate(true, _config.NotificationSettings.Templates.Telegram.AutosaveDone, _config.NotificationSettings.Templates.Telegram.AutosaveEmpty)}
                    },
                    x.GetRequiredService<ILoggerFactory>());
            });
        }

        private void ConfigureSharedFolderAccess(IServiceCollection services)
        {
            services.AddScoped<IFileStoreService>(x =>
            {
                return new WindowsShareStoreService(_config.Sm2201SpectraStoreSettings, x.GetRequiredService<ILoggerFactory>());
                //return new Smb1Service(_config.Sm2201SpectraStoreSettings, x.GetRequiredService<ILoggerFactory>());
            });
        }

        private void ConfigureRegularJobs(IServiceCollection services)
        {
            services.AddQuartz(quartz =>
            {
                quartz.UseMicrosoftDependencyInjectionJobFactory();

                Guid id = Guid.NewGuid();
                quartz.SchedulerId = id.ToString();
                quartz.SchedulerName = $"Wissance.MossbauerLab.Watcher.{id}";

                quartz.AddJob<SpectraIndexerJob>(job => job.WithIdentity(nameof(SpectraIndexerJob)));
                //todo(umv): move in config (every 3 hours)
                quartz.AddTrigger(trigger => trigger.ForJob(nameof(SpectraIndexerJob))
                      .WithCronSchedule(_config.DefaultJobsSettings.DefaultSpectraIndexerSchedule));

                quartz.AddJob<SpectraNotifyJob>(job => job.WithIdentity(nameof(SpectraNotifyJob)));
                //todo(umv): move in config (every 3 hours)
                quartz.AddTrigger(trigger => trigger.ForJob(nameof(SpectraNotifyJob))
                      //.WithSimpleSchedule(SimpleScheduleBuilder.RepeatMinutelyForever()));
                      .WithCronSchedule(_config.DefaultJobsSettings.DefaultSpectraNotifySchedule));
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
                options.AwaitApplicationStarted = true;
            });
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        private const string ApplicationConfigSectionName = "Application";

        private readonly ApplicationConfig _config;
    }
}
