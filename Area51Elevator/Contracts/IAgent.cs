using System;
using System.Threading;
using Area51Elevator.Enumerations;

namespace Area51Elevator.Contracts
{
    public interface IAgent : IDisposable
    {
        string Name { get; }

        SecurityLevel SecurityLevel { get; }

        AutoResetEvent ResetEvent { get; }

        public void StartSimulation();
    }
}
