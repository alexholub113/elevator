namespace Elevator.Component.Abstraction;

public interface IElevatorDevice
{
    event Action<int> FloorChanged;

    int Floor { get; }

    void SetDestination(int floor);

    void OpenDoor();
}