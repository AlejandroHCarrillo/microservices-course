using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse( _configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--> Connected to message Bus");


            }
            catch (Exception Ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-->Could not connet to the message Bus: { Ex.Message}");
            }
                Console.ResetColor();
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            Console.WriteLine("PublishNewPlatform");

            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("--> RabbitMQ Connection is open, sending message...");
                SendMessage(message);

            } else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("--> RabbitMQ Connection is closed, Not sending message.");

            }

            Console.ResetColor();
        }

        private void SendMessage(string message) { 
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange:"trigger", 
                                  routingKey: "",
                                  basicProperties: null,
                                  body:body);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"--> The message {message} have been sent.");
            Console.ResetColor();
        }

        private void Dispose()
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--> Message Bus Disposed");
            Console.ResetColor();
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("--> RabbitMQ connection is Shuting down");
            Console.ResetColor();
        }
    }
}
