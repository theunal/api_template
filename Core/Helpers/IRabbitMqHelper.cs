using System.Text;
using Core.Helpers;
using Core.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core.GeneralHelpers
{
    public delegate Task RabbitMqCallback(BasicDeliverEventArgs ea, object? arg);

    public interface IRabbitMqHelper
    {
        void CreateMessage(string queueName, string? body = null);
        void CreateReceived(string queue, IModel channel, RabbitMqCallback callback);
    }

    public class RabbitMqHelper : IRabbitMqHelper
    {
        public void CreateMessage(string queueName, string? body = null)
        {
            using var connection = new ConnectionFactory().CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queueName, false, false, false, null);
            channel.BasicPublish("", routingKey: queueName, body: body is not null ? Encoding.UTF8.GetBytes(body.Trim()) : null);
        }

        public void CreateReceived(string queue, IModel channel, RabbitMqCallback callback)
        {
            channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (arg, ea) => await RunCallback(queue, callback, ea, arg);
            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
        }

        private async Task RunCallback(string queue, RabbitMqCallback callback, BasicDeliverEventArgs ea, object? arg)
        {
            var body = "";
            try
            {
                var bytes = ea.Body.ToArray();
                if (bytes is not null)
                    body = Encoding.UTF8.GetString(bytes);

                await callback(ea, arg);
            }
            catch (Exception e)
            {
                GeneralStaticHelper.LogWrite($"[RABBITMQ] {queue} => \nbody: {body}", LogType.ERROR, e);
            }
        }
    }
}