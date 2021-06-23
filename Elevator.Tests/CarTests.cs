using System;
using Xunit;

namespace Elevator.Test
{
    public class CarTests
    {
        [Fact]
        public void ElevatorShouldStartWithAnEmptyFloorQueue()
        {
            var subjectUnderTest = new Car();

            Assert.Empty(subjectUnderTest.FloorQueue);
        }

        [Fact]
        public void CarShouldStartStaionary()
        {
            var subjectUnderTest = new Car();

            Assert.Equal(DirectionOfTravel.STAIONARY, subjectUnderTest.DirectionOfTravel);
            Assert.False(subjectUnderTest.Moving);
        }
    }
}
