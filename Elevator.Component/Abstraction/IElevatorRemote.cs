namespace Elevator.Component.Abstraction;

public interface IElevatorRemote
{
    event Action<int> DestinationReached;

    MoveDirection CurrentDirection { get; }

    int CurrentLevel { get; }
}