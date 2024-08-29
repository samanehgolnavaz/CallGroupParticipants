using Xunit;
using FluentAssertions;
using System.Reflection;
using NSubstitute;


namespace CallGroup.Tests
{
    public class CallGroupTests
    {

        [Fact]
        public void Constructor_ThrowsException_WhenParticipantCountIsZero()
        {
            Action act = () => new CallGroup<int>(0, _ => Task.CompletedTask, TimeSpan.FromSeconds(1));
            act.Should().Throw<ArgumentException>()
                .WithMessage("Value 0 should be greater than zero! (Parameter 'participantCount')");
        }
        [Fact]
        public void Constructor_WithValidArguments_ShouldNotThrow()
        {
            Action act = () => new CallGroup<int>(1, _ => Task.CompletedTask, TimeSpan.FromSeconds(1));
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithInvalidParticipantCount_ShouldThrow()
        {
            Action act = () => new CallGroup<int>(0, _ => Task.CompletedTask, TimeSpan.FromSeconds(1));
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_ThrowsException_WhenParticipantCountIsNegative()
        {
            Assert.Throws<ArgumentException>(() => new CallGroup<int>(-1, _ => Task.CompletedTask, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task Leave_WithCorrectParticipantCount_ShouldExecuteDelegate()
        {
            bool delegateExecuted = false;
            var callGroup = new CallGroup<int>(2, _ =>
            {
                delegateExecuted = true;
                return Task.CompletedTask;
            }, TimeSpan.FromSeconds(1));

            var task = callGroup.Join(1);
            callGroup.Leave();

            await task;

            delegateExecuted.Should().BeTrue();
        }

        [Fact]
        public void Leave_WithTooManyParticipants_ShouldThrow()
        {
            var callGroup = new CallGroup<int>(2, _ => Task.CompletedTask, TimeSpan.FromSeconds(1));

            callGroup.Leave();
            callGroup.Leave();

            Action act = () => callGroup.Leave();
            act.Should().Throw<Exception>();
        }

        [Fact]
        public async Task GroupCallDelegate_ShouldReceiveCorrectOperations()
        {
            List<int> receivedOperations = null;
            var callGroup = new CallGroup<int>(2, ops =>
            {
                receivedOperations = new List<int>(ops);
                return Task.CompletedTask;
            }, TimeSpan.FromSeconds(1));

            await Task.WhenAll(callGroup.Join(1), callGroup.Join(2));

            receivedOperations.Should().BeEquivalentTo(new List<int> { 1, 2 });
        }



        [Fact]
        public void ProcessNewJoiner_WhenCalledWithValidState_ShouldAddRequestAndSetBarrier()
        {
            // Arrange
            int participantCount = 2;
            var delegateMock = Substitute.For<Func<IReadOnlyCollection<int>, Task>>();
            delegateMock(Arg.Any<IReadOnlyCollection<int>>()).Returns(Task.CompletedTask);
            var callGroup = new CallGroup<int>(participantCount, delegateMock, TimeSpan.FromSeconds(5));

            // Access the private method using reflection
            var processNewJoinerMethod = typeof(CallGroup<int>).GetMethod("ProcessNewJoiner", BindingFlags.NonPublic | BindingFlags.Instance);

            var requests = new List<int>();
            var action = new Action(() => requests.Add(1));

            // Act
            processNewJoinerMethod.Invoke(callGroup, new object[] { action });
            processNewJoinerMethod.Invoke(callGroup, new object[] { action });

            // Assert
            requests.Should().HaveCount(2);
        }



    }
}
