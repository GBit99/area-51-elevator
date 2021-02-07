using System;
using System.Collections.Generic;
using System.Threading;
using Area51Elevator.Entities;
using Area51Elevator.Enumerations;

namespace Area51Elevator
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            // Starting the elevator in its own thread
            var elevator = new Elevator();
            var elevatorThread = new Thread(elevator.Start);

            // Letting the user decide the number of agents
            Console.Write("Enter number of agents: ");
            var numberOfAgents = int.Parse(Console.ReadLine());
            var agentThreads = new List<Thread>(numberOfAgents);

            // Start the elevator
            elevatorThread.Start();

            // Generating and starting agents
            for (int i = 1; i <= numberOfAgents; i++)
            {
                var agentSecurityLevel = (SecurityLevel)random.Next((int)SecurityLevel.Confidential, (int)SecurityLevel.TopSecret + 1);

                var agent = new Agent($"Bot {i} [{agentSecurityLevel}]", agentSecurityLevel, elevator);

                var agentThread = new Thread(agent.StartSimulation);

                agentThread.Start();

                agentThreads.Add(agentThread);
            }

            foreach (var thread in agentThreads)
            {
                thread.Join();
            }
        }
    }
}
