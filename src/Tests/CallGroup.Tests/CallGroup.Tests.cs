using Xunit;
using FluentAssertions;


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

    }
}
