using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace UdemyRabbitMQsubscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://sessjgkn:r-wEl74R5DKkLtg244A5CBIzpBsunfFD@shark.rmq.cloudamqp.com/sessjgkn");

            using var connection = factory.CreateConnection(); //bağlantı açıldı.

            var channel = connection.CreateModel(); //RabbitMQ ile haberleşmek için bir kanal oluşturuldu.

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", true/*subscriber tarafında kuyrukdaki mesaj ulaştı ise bu mesajın direk silinmesini istiyorsak true yaparız. normalde mesajın doğru işlenmeme durumu olduğundan false yapılır.*/, consumer);

            consumer.Received += (object sender,BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("Gelen Mesaj: " + message);
            };

            Console.ReadLine();
        }
    }
}
