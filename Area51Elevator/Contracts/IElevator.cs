using Area51Elevator.Enumerations;

namespace Area51Elevator.Contracts
{
    public interface IElevator
    {
        void EnterElevator(IAgent agent);

        void LeaveElevator();

        void MoveToDestination(FloorLevel destinationFloor);

        bool IsAgentAuthorized();
    }
}
