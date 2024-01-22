using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessageBusClient> _logger;

        public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RabbitMQHost"),
                Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort"))
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();


                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);


                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;


                _logger.LogInformation("--> Connected to MessageBus");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Could not connect to the Message Bus");
            }
        }


        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);


            if (_connection.IsOpen)
            {
                _logger.LogInformation("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                _logger.LogWarning("--> RabbitMQ connectionis closed, not sending");
            }
        }


        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);


            _channel.BasicPublish(exchange: "trigger",
                            routingKey: "",
                            basicProperties: null,
                            body: body);
            _logger.LogInformation($"--> We have sent {message}");
        }


        public void Dispose()
        {
            _logger.LogInformation("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }


        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("--> RabbitMQ Connection Shutdown");
        }
    }
}
