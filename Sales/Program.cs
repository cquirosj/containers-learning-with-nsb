using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Sales
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sales";
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseNServiceBus(context =>
                       {
                           var endpointConfiguration = new EndpointConfiguration("Sales");
                           endpointConfiguration.EnableInstallers();
                           
                           var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                           transport.ConnectionString("host=localhost;username=guest;password=guest");
                           transport.UseConventionalRoutingTopology();
                            
                           endpointConfiguration.SendFailedMessagesTo("error");
                           endpointConfiguration.AuditProcessedMessagesTo("audit");
                        //   endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

                           // So that when we test recoverability, we don't have to wait so long
                           // for the failed message to be sent to the error queue
                           var recoverablility = endpointConfiguration.Recoverability();
                           recoverablility.Delayed(
                               delayed =>
                               {
                                   delayed.TimeIncrease(TimeSpan.FromSeconds(2));
                               }
                           );

                        //   var metrics = endpointConfiguration.EnableMetrics();
                       //    metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

                           return endpointConfiguration;
                       });
        }
    }
}


// var endpointConfiguration = new EndpointConfiguration("Shipping");
//
// endpointConfiguration.EnableInstallers();
// endpointConfiguration.AuditProcessedMessagesTo("audit");
// endpointConfiguration.SendFailedMessagesTo("error");
//
// // var transport = endpointConfiguration.UseTransport<LearningTransport>();
// var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
// transport.UseConventionalRoutingTopology();
//
// var recover = endpointConfiguration.Recoverability();
// recover.Immediate(
//     immediate => immediate.NumberOfRetries(3)
// );
// recover.Delayed(
//     delayed => delayed.TimeIncrease(new TimeSpan(0, 0, 60))
// ); ;
// transport.ConnectionString(Environment.GetEnvironmentVariable("RabbitMQTransport_ConnectionString"));
//
// var endpointInstance = await Endpoint.Start(endpointConfiguration)
//     .ConfigureAwait(false);
//
// Console.WriteLine("Press Enter to exit.");
// Console.ReadLine();
//
// await endpointInstance.Stop()
//     .ConfigureAwait(false);