﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // Declare the exchange
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    // Declare anonymous/temporary queue
                    var queueName = channel.QueueDeclare().QueueName;

                    // Bind to the exchange
                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                    Console.WriteLine("[*] Waiting for logs...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, dlvrArgs) =>
                    {
                        var body = dlvrArgs.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] {0}", message);
                    };

                    channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
