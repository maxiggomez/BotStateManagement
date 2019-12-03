using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for CloudStorageAccount
using Microsoft.Azure.Storage.Queue; // Namespace for Queue storage types

namespace QueueOrderCmd
{
    class Program
    {
        static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
        static CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
        

        static void Main(string[] args)
        {
            // Parse the connection string and return a reference to the storage account.
            string queueName = "orders";

            CreateQueue(queueName);

            PutMessageInQueue(queueName, "Hola Mundo");

            // Solo visualizo el mensaje, no lo saco de la cola
            string message = PeekMessageQueue(queueName);
            Console.WriteLine(String.Format("Se obtuvo el mensaje:{0}",message));
            Console.ReadLine();
        }

        private static string PeekMessageQueue(string queueName)
        {
            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            
            // Peek at the next message
            CloudQueueMessage peekedMessage = queue.PeekMessage();

            // Display message.
            return peekedMessage.AsString;
        }

        private static void PutMessageInQueue(string queueName,string message)
        {
            Console.WriteLine(String.Format("Cola {0} - Agregando mensaje:{1}",queueName,message));

            CloudQueue queue = queueClient.GetQueueReference(queueName);
            // Create a message and add it to the queue.
            CloudQueueMessage queueMessage = new CloudQueueMessage(message);
            queue.AddMessage(queueMessage);

        }

        private static void CreateQueue(string queueName)
        {
            // Crear cola
            Console.WriteLine("Generadno cola:"+queueName);

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();
        }
    }
}
