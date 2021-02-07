using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Area51Elevator.Contracts;
using Area51Elevator.Entities;
using Area51Elevator.Enumerations;

namespace Area51Elevator
{
    public class Elevator : IElevator
    {
        #region Private Fields

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

        private IAgent agent;

        private FloorLevel currentFloor;

        #endregion

        #region Public Properties

        public FloorLevel? DestFloor { get; set; }

        public bool isAgentAuthorized { get; private set; }

        #endregion

        public Elevator()
        {
            currentFloor = FloorLevel.G; // initial floor level
            semaphore = new Semaphore(1, 1); // allowing only one agent inside
        }

        #region Public Methods

        public void Start()
        {
            // works non-stop
            while (true)
            {
                // if destFloor is set, someone has called the elevator
                if (DestFloor.HasValue)
                {
                    Console.WriteLine();
                    MoveToFloor(DestFloor.Value);
                }
                else
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            }
        }

        public void StartBeingUsed(IAgent agent)
        {
            semaphore.WaitOne();

            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {agent.Name} started using the elevator");

            this.agent = agent;
        }

        public void StopBeingUsed(IAgent agent)
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {agent.Name} stopped using the elevator");

            this.agent = null;

            semaphore.Release();
        }

        #endregion

        #region Private Methods

        private void MoveToFloor(FloorLevel destFloor)
        {
            // if elevator is where he has to be do nothing
            if (currentFloor == destFloor)
            {
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Elevator already on the {destFloor} floor");
                this.DestFloor = null;

                // signal the agent thread that the elevator has moved to his current floor.
                agent.ResetEvent.Set();

                return;
            }

            // elevator is moving, so we're waiting
            Thread.Sleep(SPEED_PER_FLOOR_MILLISECONDS * Math.Abs(this.currentFloor - destFloor));
            this.currentFloor = destFloor;

            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Elevator arrived on the {destFloor} floor");

            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Elevator authorizing agent {agent.Name}");
            isAgentAuthorized = authorizationDictionary.Any(e => e.Key == agent.SecurityLevel && e.Value == this.currentFloor);

            // signaling the agent that the authorization is complete
            agent.ResetEvent.Set();

            this.DestFloor = null;
        }

        #endregion
    }
}
