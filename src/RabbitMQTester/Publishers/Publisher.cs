using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RabbitMQTester.RabbitMessages;

namespace RabbitMQTester
{
    internal static class Publisher
    {
        public async static Task SendAsync(ConnectionString connection, string message)
        {
            (string host, string vHost, string username, string password) = connection;

            var appHost = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(options =>
                    {
                        options.LogToStandardErrorThreshold = LogLevel.Information;
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(configure =>
                    {
                        configure.SetKebabCaseEndpointNameFormatter();
                        configure.AddConsumer<SampleMessageConsumer>();
                        configure.UsingRabbitMq((ctx, ctr) =>
                        {
                            ctr.Host(new Uri($"rabbitmq://{host}/{vHost}"), h =>
                            {
                                h.Username(username);
                                h.Password(password);
                            });

                            ctr.ConfigureEndpoints(ctx);
                            ctr.UseMessageRetry(r => r.None());
                        });
                    });

                })
                .Build();

            try
            {
                await appHost.StartAsync();

                var bus = appHost.Services.GetRequiredService<IBusControl>();

                var rbMessage = new SampleMessage(message);
                await bus.Publish(rbMessage);

                Message.WriteResult($"Sent ({nameof(SampleMessage)}): {message}");

                //wait consumer arrive for 3 seconds
                await Task.Delay(1000);
                Console.WriteLine("Waiting...");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Message.WriteError(ex.Message);
            }
            finally
            {
                await appHost.StopAsync();
            }
        }
    }
}
