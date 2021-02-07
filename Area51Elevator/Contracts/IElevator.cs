using Area51Elevator.Enumerations;

namespace Area51Elevator.Contracts
{
    public interface IElevator
    {
        bool isAgentAuthorized { get; }

        FloorLevel? DestFloor { get; set; }

        void Start();

        void StartBeingUsed(IAgent agent);

        void StopBeingUsed(IAgent agent);

    }
}
