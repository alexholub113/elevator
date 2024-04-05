using Elevator.Component.Abstraction;
using Moq;

namespace Elevator.Tests;

public class ElevatorRemoteTests
{
    private readonly Mock<IElevatorDevice> _deviceMock = new();
}