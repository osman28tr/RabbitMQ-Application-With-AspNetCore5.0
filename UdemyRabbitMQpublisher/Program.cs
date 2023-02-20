using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQpublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://sessjgkn:r-wEl74R5DKkLtg244A5CBIzpBsunfFD@shark.rmq.cloudamqp.com/sessjgkn");

            using var connection = factory.CreateConnection(); //bağlantı açıldı.

            var channel = connection.CreateModel(); //RabbitMQ ile haberleşmek için bir kanal oluşturuldu.

            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);


            //channel.QueueDeclare("hello-queue", true/*kuyruk sadece memory'de tutulur ise false yapılır yoksa */, false/*publisher ve subscriber farklı kanallar üzerinden haberleşeceği için aynı kanal olmadığından false yapılır.*/, false/*subscriber silinir ise kuyruk da silinmesini istiyorsak true yapılır.*/); //kuyruk oluşturuldu.

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log {x}";

                var messageBody = Encoding.UTF8.GetBytes(message); //mesajı bytelar halinde göndermek avantajlı olacağından(pdf,word,excel vs.) byte ile gönderdik.

                channel.BasicPublish("logs-fanout"/*bir exchange yapısı kullanılmadığı zaman varsayılan olarak default exchange olarak adlandırılır.*/, ""/*kuyruk ismi ile aynı olmalı*/, null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiştir : {message}");
            });

            Console.ReadLine();
        }
    }
}
