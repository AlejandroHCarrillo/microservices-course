using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProssesor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProssesor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ() { 
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse( _configuration["RabbitMQPort"])};
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--> Listening on the Message Bus...");
            Console.ResetColor();

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("--> Event received!");
                Console.ResetColor();

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProssesor.ProcessEvent(notificationMessage);

            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("--> Connection Shutting down...");
            Console.ResetColor();
        }

        public override void Dispose()
        {
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
    }
}
