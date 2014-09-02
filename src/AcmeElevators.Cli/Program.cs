using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AcmeElevators.v2;

namespace AcmeElevators.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numberOfFloors = 1002;
            const int numberOfElevators = 16;
            const int numberOfRequests = 10 * 1000;
            var pickupCount = 0;
            var stepCount = 0;
            var random = new Random();
            IElevatorControlSystem system = new ControlSystem(numberOfElevators);


            while (pickupCount < numberOfRequests)
            {
                var originatingFloor = random.Next(1, numberOfFloors + 1);
                var destinationFloor = random.Next(1, numberOfFloors + 1);
                if (originatingFloor != destinationFloor)
                {
                    system.Pickup(originatingFloor, destinationFloor);
                    pickupCount++;
                }
            }

            while (system.AnyOutstandingPickups())
            {
                system.Step();
                stepCount++;
            }

            Console.WriteLine("Transported {0} elevator riders to their requested destinations in {1} steps.", pickupCount, stepCount);
            Console.ReadLine();









        }
    }
}
