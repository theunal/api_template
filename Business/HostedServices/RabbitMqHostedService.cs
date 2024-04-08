using Core.GeneralHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Business.HostedServices
{
    public class RabbitMqHostedService(IServiceScopeFactory _serviceScopeFactory, IRabbitMqHelper rabbitMqHelperRepository) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = new ConnectionFactory().CreateConnection();
            var channel = connection.CreateModel();

            //    //rabbitMqHelperRepository.CreateReceived("queue_name", channel, async (ea, arg) => await Task.Run(() =>
            //    //{
            //    //    var body = JsonConvert.DeserializeObject<object>(Encoding.UTF8.GetString(ea.Body.ToArray()));
            //    //    var provider = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ServiceName>();
            //    //    provider.MethodName(body);
            //    //}));

            return Task.CompletedTask;
        }
    }
}