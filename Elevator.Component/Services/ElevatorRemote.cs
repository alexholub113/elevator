using Elevator.Component.Abstraction;

namespace Elevator.Component.Services;

public class ElevatorRemote : IElevatorExternalRemote, IElevatorInternalRemote
{
    private readonly IElevatorDevice _elevatorDevice;

    public ElevatorRemote(IElevatorDevice elevatorDevice)
    {
        _elevatorDevice = elevatorDevice;
        _elevatorDevice.FloorChanged += HandleFloorChanged;
    }

    public event Action<int>? FloorChanged;

    public MoveDirection MoveDirection { get; } = MoveDirection.None;

    public int Floor { get; } = 0;

    public void SetDestination(int floor)
    {
        throw new NotImplementedException();
    }

    public void Call(int level, DestinationDirection direction)
    {
        throw new NotImplementedException();
    }

    private void HandleFloorChanged(int floor)
    {
        throw new NotImplementedException();
    }

    protected virtual void OnFloorChanged(int obj)
    {
        FloorChanged?.Invoke(obj);
    }
}