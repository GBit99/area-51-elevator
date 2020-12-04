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
            var elevator = new Elevator();

            var numberOfAgents = int.Parse(args[0]);
            var random = new Random();

            var agentThreads = new List<Thread>(numberOfAgents);

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
