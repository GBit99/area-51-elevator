using System;
using System.Threading.Tasks;
using Area51Elevator.Contracts;
using Area51Elevator.Enumerations;

namespace Area51Elevator.Entities
{
    public class Agent : IAgent
    {
        #region Private Fields

        private Elevator elevator;
        private bool wantsToLeave;
        private readonly Random random;
        private FloorLevel currentFloor;

        #endregion

        public Agent(string name, SecurityLevel securityLevel, Elevator elevator)
        {
            currentFloor = FloorLevel.G; // The initial floor
            this.Name = name;
            this.elevator = elevator;
            this.securityLevel = securityLevel;
            random = new Random();
        }

        #region Public Properties

        public string Name { get; private set; }

        public SecurityLevel securityLevel { get; private set; }

        #endregion

        #region Public Methods

        public void StartSimulation()
        {
            Console.WriteLine($"Agent {Name} just arrived at the Area 51 Base. He's on the ground floor.");

            do
            {
                var action = random.Next(1, 4);

                switch (action)
                {
                    case 1: LookAround(); break;
                    case 2: UseElevator(); break;
                    case 3: DecideToLeaveBase(); break;
                }
            } while (!this.wantsToLeave);

            // Leave base
            if (currentFloor != FloorLevel.G)
            {
                UseElevator(FloorLevel.G);
            }

            Console.WriteLine($"Agent {Name} is leaving the base.");
        }

        #endregion

        #region Private Methods

        private void UseElevator(FloorLevel? floorDecision = null)
        {
            // Call the elevator
            Console.WriteLine($"Agent {Name} calling the elevator to the {currentFloor} floor...");
            elevator.MoveToDestination(currentFloor);

            // Enter only if there is no one inside
            if (!elevator.HasAgentInside)
            {
                elevator.EnterElevator(this);

                var destinationFloor = floorDecision ?? ChooseDestination();
                elevator.MoveToDestination(destinationFloor);
                currentFloor = destinationFloor;

                // Go to different random floors until you have access for the floor
                while (!elevator.IsAgentAuthorized())
                {
                    destinationFloor = ChooseDestination();
                    elevator.MoveToDestination(destinationFloor);
                    currentFloor = destinationFloor;
                }

                elevator.LeaveElevator();
            }
            else
            {
                Console.WriteLine($"Agent {Name} can't enter the elevator because there's a person inside.");
            }
        }

        private void LookAround()
        {
            Console.WriteLine($"Agent {Name} is looking around on {currentFloor} floor.");

            Task.Delay(500).Wait();
        }

        private void DecideToLeaveBase()
        {
            Console.WriteLine($"Agent {Name} has decided to leave the base...");
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
