using Elevator.Component.Abstraction;
using Elevator.Component.Comparers;

namespace Elevator.Component.Services;

/// <summary>
/// Controller logic:
/// - When a call is made, the controller will add the call to the appropriate queue.
/// - The controller will then move the elevator to the next destination.
/// - When the elevator reaches a destination, the controller will remove the destination from the queue and move to the next destination.
/// </summary>
public class SimpleElevatorController : IElevatorInternalRemote, IElevatorExternalRemote
{
    private readonly IElevator _elevator;
    private readonly PriorityQueue<int, int> _downQueue = new(new DescendingComparer());
    private readonly PriorityQueue<int, int> _upQueue = new(new AscendingComparer());
    private readonly object _lockObject = new();

    public SimpleElevatorController(IElevator elevator)
    {
        _elevator = elevator;
        CurrentFloor = _elevator.CurrentFloor;
        _elevator.DestinationReached += HandleDestinationReached;
        _elevator.FloorChanged += level => CurrentFloor = level;
    }

    public event Action<int>? DestinationReached;

    public MoveDirection CurrentDirection { get; private set; } = MoveDirection.None;

    public int CurrentFloor { get; private set; }

    public void Call(int floor)
    {
        lock (_lockObject)
        {
            SetDestination(floor);
        }
    }

    public void SetDestination(int floor)
    {
        lock (_lockObject)
        {
            if (CurrentFloor == floor)
            {
                _elevator.OpenDoor();
                return;
            }

            if (CurrentFloor < floor)
            {
                _upQueue.Enqueue(floor, floor);
            }
            else
            {
                _downQueue.Enqueue(floor, floor);
            }

            MoveNext();
        }
    }

    private void MoveNext()
    {
        if (CurrentDirection == MoveDirection.None)
        {
            if (_upQueue.Count > 0)
            {
                CurrentDirection = MoveDirection.Up;
                _elevator.SetDestination(_upQueue.Peek());
            }
            else if (_downQueue.Count > 0)
            {
                CurrentDirection = MoveDirection.Down;
                _elevator.SetDestination(_downQueue.Peek());
            }
        }
        else if (CurrentDirection == MoveDirection.Down)
        {
            if (_downQueue.Count > 0)
            {
                _elevator.SetDestination(_downQueue.Peek());
            }
            else
            {
                CurrentDirection = MoveDirection.None;
                MoveNext();
            }
        }
        else
        {
            if (_upQueue.Count > 0)
            {
                _elevator.SetDestination(_upQueue.Peek());
            }
            else
            {
                CurrentDirection = MoveDirection.None;
                MoveNext();
            }
        }
    }

    private void HandleDestinationReached(int floor)
    {
        lock (_lockObject)
        {
            if (CurrentDirection == MoveDirection.None) return;

            try
            {
                if (CurrentDirection == MoveDirection.Down)
                {
                    if (floor != _downQueue.Peek())
                    {
                        throw new InvalidOperationException("Controller out of sync");
                    }

                    _downQueue.Dequeue();
                }
                else
                {
                    if (floor != _upQueue.Peek())
                    {
                        throw new InvalidOperationException("Controller out of sync");
                    }

                    _upQueue.Dequeue();
                }

                OnDestinationReached(floor);
                MoveNext();
            }
            finally
            {
                _elevator.OpenDoor();
            }
        }
    }

    private void OnDestinationReached(int obj)
    {
        DestinationReached?.Invoke(obj);
    }
}