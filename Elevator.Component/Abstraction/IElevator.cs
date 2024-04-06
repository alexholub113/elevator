namespace Elevator.Component.Abstraction;

public interface IElevator
{
    int CurrentFloor { get; }

    event Action<int> DestinationReached;

    event Action<int> FloorChanged;

    void SetDestination(int floor);

    void OpenDoor();
}