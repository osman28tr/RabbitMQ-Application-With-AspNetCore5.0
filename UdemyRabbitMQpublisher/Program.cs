using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQpublisher
{
    public enum LogNames
    {
        Critical=1,
        Error=2,
        Warning=3,
        Info=4
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://sessjgkn:r-wEl74R5DKkLtg244A5CBIzpBsunfFD@shark.rmq.cloudamqp.com/sessjgkn");

            using var connection = factory.CreateConnection(); //bağlantı açıldı.

            var channel = connection.CreateModel(); //RabbitMQ ile haberleşmek için bir kanal oluşturuldu.

            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var queueName = $"direct-queue-{x}";

                var routeKey = $"route-{x}";

                channel.QueueDeclare(queueName, true, false, false);

                channel.QueueBind(queueName, "logs-direct", routeKey,null
                    );
            });



            //channel.QueueDeclare("hello-queue", true/*kuyruk sadece memory'de tutulur ise false yapılır yoksa */, false/*publisher ve subscriber farklı kanallar üzerinden haberleşeceği için aynı kanal olmadığından false yapılır.*/, false/*subscriber silinir ise kuyruk da silinmesini istiyorsak true yapılır.*/); //kuyruk oluşturuldu.

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames)new Random().Next(1, 5);

                string message = $"log-type: {log}"; 

                var messageBody = Encoding.UTF8.GetBytes(message); //mesajı bytelar halinde göndermek avantajlı olacağından(pdf,word,excel vs.) byte ile gönderdik.

                var routeKey = $"route-{log}";

                channel.BasicPublish("logs-direct",routeKey,/*bir exchange yapısı kullanılmadığı zaman varsayılan olarak default exchange olarak adlandırılır.*//*kuyruk ismi ile aynı olmalı*/ null, messageBody);

                Console.WriteLine($"Log gönderilmiştir : {message}");
            });

            Console.ReadLine();
        }
    }
}
