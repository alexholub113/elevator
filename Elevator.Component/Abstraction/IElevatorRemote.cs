namespace Elevator.Component.Abstraction;

public interface IElevatorRemote
{
    event Action<int> FloorChanged;

    MoveDirection MoveDirection { get; }

    int Floor { get; }
}