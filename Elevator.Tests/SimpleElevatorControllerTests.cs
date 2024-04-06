using Elevator.Component.Abstraction;
using Elevator.Component.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Elevator.Tests;

public class SimpleElevatorControllerTests
{
    private readonly Mock<IElevator> _elevatorMock;
    private readonly SimpleElevatorController _controller;

    public SimpleElevatorControllerTests()
    {
        _elevatorMock = new Mock<IElevator>();
        _controller = new SimpleElevatorController(_elevatorMock.Object);
    }

    [Fact]
    public void MultipleCalls_SetDestinationCalledForNextFloor()
    {
        _controller.SetDestination(1);
        _controller.SetDestination(2);
        _controller.SetDestination(3);

        _elevatorMock.Verify(e => e.SetDestination(1), Times.Exactly(3));
    }

    [Fact]
    public void MultipleCalls_ElevatorMovedUp_ValidOrder()
    {
        var actualOrder = new List<int>();
        _controller.DestinationReached += level => actualOrder.Add(level);
        _controller.CurrentDirection.Should().Be(MoveDirection.None);

        // Initially, move up
        _controller.SetDestination(1);
        _controller.SetDestination(5);
        _controller.SetDestination(2);
        _controller.SetDestination(4);
        _elevatorMock.Verify(e => e.SetDestination(2), Times.Never);
        _elevatorMock.Verify(e => e.SetDestination(1));
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);

        // Simulate reaching 1st floor
        _elevatorMock.Raise(m => m.FloorChanged += null, 1);
        _elevatorMock.Raise(m => m.DestinationReached += null, 1);
        actualOrder.Should().BeEquivalentTo(new[] { 1 });
        _elevatorMock.Verify(e => e.SetDestination(2), Times.Once);
        _controller.CurrentFloor.Should().Be(1);
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);

        // Simulate reaching 2nd floor
        _elevatorMock.Raise(m => m.FloorChanged += null, 2);
        _elevatorMock.Raise(m => m.DestinationReached += null, 2);
        actualOrder.Should().BeEquivalentTo(new[] { 1, 2 });
        _elevatorMock.Verify(e => e.SetDestination(4), Times.Once);
        _controller.CurrentFloor.Should().Be(2);
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);

        // Simulate reaching 3nd floor
        _elevatorMock.Raise(m => m.FloorChanged += null, 3);
        _controller.CurrentFloor.Should().Be(3);

        // Simulate reaching 4th floor
        _elevatorMock.Raise(m => m.FloorChanged += null, 4);
        _elevatorMock.Raise(m => m.DestinationReached += null, 4);
        actualOrder.Should().BeEquivalentTo(new[] { 1, 2, 4 });
        _elevatorMock.Verify(e => e.SetDestination(5), Times.Once);
        _controller.CurrentFloor.Should().Be(4);
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);

        // Simulate reaching 5th floor
        _elevatorMock.Raise(m => m.FloorChanged += null, 5);
        _elevatorMock.Raise(m => m.DestinationReached += null, 5);
        actualOrder.Should().BeEquivalentTo(new[] { 1, 2, 4, 5 });
        _controller.CurrentFloor.Should().Be(5);
        _controller.CurrentDirection.Should().Be(MoveDirection.None);
    }

    [Fact]
    public void DirectionChange_ElevatorMovesUpThenDown_ValidOrder()
    {
        var actualOrder = new List<int>();
        _controller.DestinationReached += level => actualOrder.Add(level);
        _controller.CurrentDirection.Should().Be(MoveDirection.None);

        // Initially, move up
        _controller.SetDestination(3);
        _controller.SetDestination(6); // The elevator will start moving up to 3, then 6
        _elevatorMock.Verify(e => e.SetDestination(3));
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);

        // Simulate reaching 3rd floor, then set a new destination down
        _elevatorMock.Raise(m => m.FloorChanged += null, 1);
        _elevatorMock.Raise(m => m.FloorChanged += null, 2);
        _elevatorMock.Raise(m => m.FloorChanged += null, 3);
        _elevatorMock.Raise(m => m.DestinationReached += null, 3);
        actualOrder.Should().BeEquivalentTo(new[] { 3 });
        _controller.CurrentDirection.Should().Be(MoveDirection.Up);
        _controller.SetDestination(1); // Now, there's a request below the current level
        _elevatorMock.Verify(e => e.SetDestination(6));

        // Complete moving up first
        _elevatorMock.Raise(m => m.FloorChanged += null, 4);
        _elevatorMock.Raise(m => m.FloorChanged += null, 5);
        _elevatorMock.Raise(m => m.FloorChanged += null, 6);
        _elevatorMock.Raise(m => m.DestinationReached += null, 6);
        actualOrder.Should().BeEquivalentTo(new[] { 3, 6 });
        _controller.CurrentDirection.Should().Be(MoveDirection.Down); // Now, it should change direction
        _controller.CurrentFloor.Should().Be(6);
        _elevatorMock.Verify(e => e.SetDestination(1), Times.Once);

        // Now, the elevator should move down to 1
        _elevatorMock.Raise(m => m.FloorChanged += null, 5);
        _elevatorMock.Raise(m => m.FloorChanged += null, 4);
        _elevatorMock.Raise(m => m.FloorChanged += null, 3);
        _elevatorMock.Raise(m => m.FloorChanged += null, 2);
        _elevatorMock.Raise(m => m.FloorChanged += null, 1);
        _elevatorMock.Verify(e => e.SetDestination(1), Times.Once);
        _elevatorMock.Raise(m => m.DestinationReached += null, 1);
        actualOrder.Should().BeEquivalentTo(new[] { 3, 6, 1 });
        _controller.CurrentDirection.Should().Be(MoveDirection.None);
        _controller.CurrentFloor.Should().Be(1);
    }
}