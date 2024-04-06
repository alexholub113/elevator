namespace Elevator.Component.Abstraction;

public interface ISmartElevatorController : ISimpleElevatorController
{
    void Call(int level, DestinationDirection direction);
}