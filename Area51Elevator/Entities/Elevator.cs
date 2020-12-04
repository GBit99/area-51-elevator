using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Area51Elevator.Contracts;
using Area51Elevator.Enumerations;

namespace Area51Elevator
{
    public class Elevator : IElevator
    {
        #region Private fields

        private const int SPEED_PER_FLOOR_MILLISECONDS = 1000;
        private readonly List<KeyValuePair<SecurityLevel, FloorLevel>> authorizationDictionary = new List<KeyValuePair<SecurityLevel, FloorLevel>>()
        {
            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.Confidential, FloorLevel.G),

            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.Secret, FloorLevel.G),
            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.Secret, FloorLevel.S),

            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.TopSecret, FloorLevel.G),
            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.TopSecret, FloorLevel.S),
            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.TopSecret, FloorLevel.T1),
            new KeyValuePair<SecurityLevel, FloorLevel>(SecurityLevel.TopSecret, FloorLevel.T2)
        };

        private readonly Semaphore semaphore;

        private volatile IAgent agent;

        private volatile FloorLevel currentFloor;

        #endregion

        public Elevator()
        {
            currentFloor = FloorLevel.G;
            semaphore = new Semaphore(1, 1);
        }

        public bool HasAgentInside => this.agent != null;

        #region Public Methods

        public void EnterElevator(IAgent agent)
        {
            this.agent = agent;

            Console.WriteLine($"Agent {agent.Name} has entered the elevator.");

            semaphore.WaitOne();
        }

        public void LeaveElevator()
        {
            Console.WriteLine($"Agent {agent.Name} is leaving the elevator.");

            this.agent = null;

            semaphore.Release();
        }

        public void MoveToDestination(FloorLevel destinationFloor)
        {
            if (!HasAgentInside)
            {
                semaphore.WaitOne();
            }

            if (currentFloor == destinationFloor)
            {
                Console.WriteLine($"Elevator is already on the {destinationFloor} floor");
            }
            else
            {
                Console.WriteLine($"Elevator moving to {destinationFloor} floor...");

                var floorDifference = Math.Abs(currentFloor - destinationFloor);

                Task.Delay(SPEED_PER_FLOOR_MILLISECONDS * floorDifference).Wait();

                Console.WriteLine($"Elevator reached floor {destinationFloor} ...");

                currentFloor = destinationFloor;
            }

            if (!HasAgentInside)
            {
                semaphore.Release();
            }
        }

        public bool IsAgentAuthorized()
        {
            Console.WriteLine($"Authorizing agent {agent.Name}");

            if (authorizationDictionary.Any(e => e.Key == agent.securityLevel && e.Value == currentFloor))
            {
                Console.WriteLine($"Agent {agent.Name} authorized successfully");
                return true;
            }

            Console.WriteLine($"Agent {agent.Name} authorization failed.");
            return false;
        }

        #endregion
    }
}
