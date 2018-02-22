using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace CreateDeviceIdentity {
    class Program {
        static RegistryManager registryManager;
        // Replace the placeholer with your Azure IoT Hub connection string
        static string connectionString = "<connection string>";

        static void Main(string[] args) {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsyn().Wait();
            Console.ReadLine();
        }

        // the device will be registered on the IoT-Hub
        private static async Task AddDeviceAsyn() {
            string deviceId = "firstDevice";
            Device device;
            try {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            } catch (DeviceAlreadyExistsException) {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
