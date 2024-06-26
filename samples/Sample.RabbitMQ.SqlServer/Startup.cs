﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.RabbitMQ.SqlServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -e "MSSQL_PID=Evaluation" -p 1433:1433  --name sqlpreview --hostname sqlpreview -d mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04
            services.AddDbContext<AppDbContext>();

            //services
            //    .AddSingleton<IConsumerServiceSelector, TypedConsumerServiceSelector>()
            //    .AddQueueHandlers(typeof(Startup).Assembly);

            services.AddCap(x =>
            {
                x.UseEntityFramework<AppDbContext>();
                x.UseRabbitMQ("localhost");
                x.UseDashboard();
        
                x.EnablePublishParallelSend = true;
                
                //x.FailedThresholdCallback = failed =>
                //{
                //    var logger = failed.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                //    logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                //        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                //};
                //x.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
