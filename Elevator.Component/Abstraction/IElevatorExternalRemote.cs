namespace Elevator.Component.Abstraction;

public interface IElevatorExternalRemote : IElevatorRemote
{
    void Call(int level, DestinationDirection direction);
}