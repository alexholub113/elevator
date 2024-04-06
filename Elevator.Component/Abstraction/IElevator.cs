namespace Elevator.Component.Abstraction;

public interface IElevator
{
    event Action<int> DestinationReached;

    int CurrentLevel { get; }

    void SetDestination(int floor);

    void OpenDoor();
}