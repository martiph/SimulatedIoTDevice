using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace ReadDeviceToCloudMessages {
    class Program {

        // Replace the placeholder with your Azure IoT Hub connection string
        static string connectionString = "<connection string>";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct) {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);//only data is processed, which was created after starting the application
            while (true) {
                if (ct.IsCancellationRequested) { break; }
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) { continue; }
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition {0} Data: {1}", partition, data);
            }
        }

        static void Main(string[] args) {
            Console.WriteLine("Receive Messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            CancellationTokenSource cts = new CancellationTokenSource();

            System.Console.CancelKeyPress += (s, e) => {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions) {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}
