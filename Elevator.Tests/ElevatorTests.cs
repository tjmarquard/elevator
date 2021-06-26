namespace Elevator.Tests
{
    using Xunit;

    public class ElevatorTests
    {
        [Fact]
        public void ElevatorShouldHaveFiveFloors()
        {
            var subjectUnderTest = new Elevator(5);

            Assert.Collection<int>(
                subjectUnderTest.FloorNumbers,
                floorNumber => Assert.Equal(1, floorNumber),
                floorNumber => Assert.Equal(2, floorNumber),
                floorNumber => Assert.Equal(3, floorNumber),
                floorNumber => Assert.Equal(4, floorNumber),
                floorNumber => Assert.Equal(5, floorNumber));
        }

        [Theory]
        [InlineData("4D", 4)]
        [InlineData("4U", 4)]
        [InlineData("5U", 5)]
        [InlineData("1U", 1)]
        [InlineData("1D", 1)]
        [InlineData("3D4", 3)]
        [InlineData("3D4U", 3)]
        [InlineData("3", 3)]
        [InlineData("D3", 3)]
        public void GetEnteredFloorNumberShouldBeValid(string enteredButton, int expectedFloorNumber)
        {
            var actualFloorNumber = Elevator.GetEnteredFloorNumber(enteredButton);

            Assert.Equal(expectedFloorNumber, actualFloorNumber);
        }

        [Theory]
        [InlineData("D", 0)]
        public void GetEnteredFloorNumberShouldBeInvalid(string enteredButton, int expectedFloorNumber)
        {
            var actualFloorNumber = Elevator.GetEnteredFloorNumber(enteredButton);

            Assert.Equal(expectedFloorNumber, actualFloorNumber);
        }

        [Theory]
        [InlineData("5D", DirectionOfTravel.DOWN)]
        [InlineData("4U", DirectionOfTravel.UP)]
        [InlineData("2B", DirectionOfTravel.NONE)]
        [InlineData("2", DirectionOfTravel.NONE)]
        [InlineData("2u", DirectionOfTravel.UP)]
        public void EnteredDirectionOfTravelShouldBeValid(string enteredButton, DirectionOfTravel expectedDirection)
        {
            var subjectUnderTest = new Elevator(5);
            var actualDirection = subjectUnderTest.GetEnteredDirectionOfTravel(enteredButton);

            Assert.Equal(expectedDirection, actualDirection);
        }

        [Fact]
        public async void ProcessButtonPressesTest()
        {
            var subjectUnderTest = new Elevator(3);
            var expectedLastFloor = 2;

            var buttonPress2Down = new ButtonPress()
            {
                FloorNumber = 2,
                DirectionOfTravel = DirectionOfTravel.DOWN,
            };
            var buttonPress2Up = new ButtonPress()
            {
                FloorNumber = 2,
                DirectionOfTravel = DirectionOfTravel.UP,
            };
            var buttonPress3None = new ButtonPress()
            {
                FloorNumber = 3,
                DirectionOfTravel = DirectionOfTravel.NONE,
            };

            subjectUnderTest.Car.ButtonPresses.Add(buttonPress2Down);
            subjectUnderTest.Car.ButtonPresses.Add(buttonPress2Up);
            subjectUnderTest.Car.ButtonPresses.Add(buttonPress3None);

            await subjectUnderTest.ProcessButtonPresses();

            Assert.Equal(expectedLastFloor, subjectUnderTest.Car.CurrentFloor);
        }
    }
}
