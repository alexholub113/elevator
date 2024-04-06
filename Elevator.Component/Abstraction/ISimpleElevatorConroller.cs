namespace Elevator.Component.Abstraction;

public interface ISimpleElevatorController
{
    void SetDestination(int level);

    event Action<int> DestinationReached;

    MoveDirection CurrentDirection { get; }

    int CurrentFloor { get; }
}