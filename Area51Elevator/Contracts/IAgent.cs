using Area51Elevator.Enumerations;

namespace Area51Elevator.Contracts
{
    public interface IAgent
    {
        string Name { get; }

        SecurityLevel securityLevel { get; }

        void StartSimulation();
    }
}
