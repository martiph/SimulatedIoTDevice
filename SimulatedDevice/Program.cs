using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice {
    class Program {

        static DeviceClient deviceClient;
        // Replace the placeholder for the hostname
        static string iotHubUri = "<hostname>";
        // Replace the placeholder with your device key (Created with "CreateDeviceIdentity")
        static string deviceKey = "<device key>";

        private static async void SendDeviceToCloudMessagesAsync() {
            double minTemperature = 20;
            double minHumidity = 60;
            int messageId = 1;
            Random rand = new Random();

            while (true) {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new {
                    messageId = messageId++,
                    deviceId = "firstDevice",
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }

        static void Main(string[] args) {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("firstDevice", deviceKey), TransportType.Mqtt);
            deviceClient.ProductInfo = "HappyPath_Simulated-CSharp";
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
