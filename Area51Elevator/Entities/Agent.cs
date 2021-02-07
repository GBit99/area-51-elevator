using System;
using System.Threading;
using System.Threading.Tasks;
using Area51Elevator.Contracts;
using Area51Elevator.Enumerations;

namespace Area51Elevator.Entities
{
    public class Agent : IAgent
    {
        #region Private Fields

        private IElevator elevator;
        private bool wantsToLeave;
        private readonly Random random;
        private FloorLevel currentFloor;

        #endregion

        public Agent(string name, SecurityLevel securityLevel, IElevator elevator)
        {
            currentFloor = FloorLevel.G; // The initial floor
            this.Name = name;
            this.elevator = elevator;
            this.SecurityLevel = securityLevel;
            this.ResetEvent = new AutoResetEvent(false);
            random = new Random();
        }

        #region Public Properties

        public AutoResetEvent ResetEvent { get; private set; }

        public string Name { get; private set; }

        public SecurityLevel SecurityLevel { get; private set; }

        #endregion

        #region Public Methods

        public void StartSimulation()
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} arrived at G floor of the Area 51 Base");

            do
            {
                //var action = 2;
                var action = random.Next(1, 4);

                switch (action)
                {
                    case 1: LookAround(); break;
                    case 2:
                        {
                            var destFloor = ChooseDestination();
                            UseElevator(destFloor);
                            break;
                        }
                    case 3: DecideToLeaveBase(); break;
                }
            } while (!this.wantsToLeave);

            // Agent has decided to leave
            if (currentFloor != FloorLevel.G)
            {
                UseElevator(FloorLevel.G);
            }

            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} is leaving the base.");
        }

        public void Dispose()
        {
            try
            {
                this.ResetEvent.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Private Methods

        private void UseElevator(FloorLevel destFloor, bool isAgentAlreadyInside = false)
        {
            if (!isAgentAlreadyInside)
            {
                // Make sure only you're working with the elevator at the moment
                elevator.StartBeingUsed(this);

                // 1. Call and wait for the elevator
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} calling the elevator to the {currentFloor} floor");
                //elevator.agentWaitingForElevator = this;

                // 2. select the current floor as a destination for the elevator
                elevator.DestFloor = currentFloor;

                // 3. wait for the elevator to come to the current agent floor
                this.ResetEvent.WaitOne();

                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} entering the elevator");
            }

            // 4. Press the button for the destFloor
            elevator.DestFloor = destFloor;
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} pressed the button for {destFloor} floor");

            // 5. wait for the elevator to move to the specific floor
            ResetEvent.WaitOne();

            // 6. set the current floor to the destionation floor
            currentFloor = destFloor;

            // 7. Check if you've been authorized
            if (elevator.isAgentAuthorized)
            {
                Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} AUTHORIZED and left the elevator");

                // 8. Leave elevator
                elevator.StopBeingUsed(this);
            }
            else
            {
                UnauthorizedProcedure();
            }
        }

        private void UnauthorizedProcedure()
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} UNAUTHORIZED for the current floor");

            // if agent is unauthorized choose another destination
            var destFloor = ChooseDestination();

            // and use the elevator
            UseElevator(destFloor, isAgentAlreadyInside: true);
        }

        private void LookAround()
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} is looking around on {currentFloor} floor.");

            Task.Delay(500).Wait();
        }

        private void DecideToLeaveBase()
        {
            Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Agent {Name} has decided to leave the base...");
            this.wantsToLeave = true;
        }

        private FloorLevel ChooseDestination()
        {
            FloorLevel selectedFloor;

            do
            {
                selectedFloor = (FloorLevel)random.Next((int)FloorLevel.G, (int)FloorLevel.T2 + 1);
            } while (selectedFloor == currentFloor);

            return selectedFloor;
        }

        #endregion
    }
}
