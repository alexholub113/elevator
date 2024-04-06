namespace Elevator.Component.Abstraction;

public interface IElevatorExternalDirectionalRemote : IElevatorRemote
{
    void Call(int level, DestinationDirection direction);
}