namespace Elevator.Component.Abstraction;

public interface IElevatorInternalRemote : IElevatorRemote
{
    void SetDestination(int level);
}