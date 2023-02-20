using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

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

            //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            var randomQueueName = channel.QueueDeclare().QueueName; //her subscriber'ın farklı kuyruk ismi olması açısından random bi kuyruk ismi verildi.

            //channel.QueueDeclare(randomQueueName, true, false, false);//kuyruk subscriber down olursa silinmesin.

            channel.QueueBind(randomQueueName,"logs-fanout","",null);//declare edilmedi bind edildi. yani subscriber down olursa kuyruk da silinir. Declare edilirse ilgili subscriber down olursa kuyruk silinmez durur.


            channel.BasicQos(0, 1, false/*true olur ise toplamda 2. parametredeki değer kadar olacak şekilde aynı anda bütün subscriberlar'a gönderir fakat false olur ise tek seferde bütün subscriberlar'a 2. parametredeki değer kadar mesajları gönderir.*/);

            var consumer = new EventingBasicConsumer(channel); //subscriber oluşturuldu.

            channel.BasicConsume(randomQueueName, false/*subscriber tarafında kuyrukdaki mesaj ulaştı ise bu mesajın direk silinmesini istiyorsak true yaparız. normalde mesajın doğru işlenmeme durumu olduğundan false yapılır.*/, consumer);

            Console.WriteLine("Loglar dinleniyor...");

            consumer.Received += (object sender,BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("Gelen Mesaj: " + message);

                channel.BasicAck(e.DeliveryTag, false); //rabbitmq ilgili mesajın durumundan haberdar edildi.
            };

            Console.ReadLine();
        }
    }
}
