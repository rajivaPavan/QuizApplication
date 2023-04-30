using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace QuizApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config =>
                    {
                        //Retrieve the Connection String from the secrets manager
                        IConfiguration settings = config.Build();
                        var connectionString = settings.GetConnectionString("AppConfig");

                        // Load configuration from Azure App Configuration
                        config.AddAzureAppConfiguration(options =>
                        {
                            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                            var azureAppConfigurationOptions = options.Connect(connectionString)
                                // Load all keys and have no label
                                .Select("*");

                            azureAppConfigurationOptions 
                                // Configure to reload configuration if the registered sentinel key is modified
                                .ConfigureRefresh(refreshOptions =>
                                    refreshOptions.Register("*", refreshAll: true));

                            // Load all feature flags with no label
                            options.UseFeatureFlags();
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}