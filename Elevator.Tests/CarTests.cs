using System;
using Xunit;

namespace Elevator.Test
{
    public class CarTests
    {
        [Fact]
        public void ElevatorShouldStartWithAnEmptyFloorQueue()
        {
            var subjectUnderTest = new Car(5);

            Assert.Empty(subjectUnderTest.FloorQueue);
        }

        [Fact]
        public void CarShouldStartStaionary()
        {
            var subjectUnderTest = new Car(5);

            Assert.Equal(DirectionOfTravel.STAIONARY, subjectUnderTest.DirectionOfTravel);
            Assert.False(subjectUnderTest.Moving);
        }

        [Fact]
        public void CarShouldHaveFiveFloors()
        {
            var subjectUnderTest = new Car(5);

            Assert.Collection<int>(subjectUnderTest.Floors, 
                floor => Assert.Equal(1, floor),
                floor => Assert.Equal(2, floor),
                floor => Assert.Equal(3, floor),
                floor => Assert.Equal(4, floor),
                floor => Assert.Equal(5, floor)
                );
        }

        [Fact]
        public void CarShouldStartOnFirstFloor()
        {
            var subjectUnderTest = new Car(5);

            Assert.Equal(1, subjectUnderTest.CurrentFloor);
        }

        [Fact]
        public void CarHasAMaxWeightLimitOf2000()
        {
            var subjectUnderTest = new Car(5);

            Assert.Equal(2000, subjectUnderTest.MaxWeightLimit);
        }

        [Fact]
        public void CarIsUnderMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit - 1;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
        }

        [Fact]
        public void CarIsOverMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit + 1;

            Assert.False(subjectUnderTest.IsUnderMaxWeightLimit);
        }

        [Fact]
        public void CarIsAdMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
        }
    }
}
