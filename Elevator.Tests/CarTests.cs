namespace Elevator.Tests
{
    using ChanceNET;
    using Serilog;
    using Xunit;

    public class CarTests
    {
        private readonly ILogger logger;
        private readonly Chance chance;

        public CarTests()
        {
            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/elevator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            chance = new Chance();
        }

        [Fact]
        public void ElevatorShouldStartWithAnEmptyFloorQueue()
        {
            var subjectUnderTest = new Car(logger);

            Assert.Empty(subjectUnderTest.ButtonPresses);
        }

        [Fact]
        public void CarShouldStartStaionary()
        {
            var subjectUnderTest = new Car(logger);

            Assert.Equal(DirectionOfTravel.NONE, subjectUnderTest.DirectionOfTravel);
            Assert.False(subjectUnderTest.IsMoving);
        }

        [Fact]
        public void CarShouldStartOnFirstFloor()
        {
            var subjectUnderTest = new Car(logger);

            Assert.Equal(1, subjectUnderTest.CurrentFloor);
        }

        [Fact]
        public void CarHasAMaxWeightLimitOf2000()
        {
            var subjectUnderTest = new Car(logger);

            Assert.Equal(2000, subjectUnderTest.MaxWeightLimit);
        }

        [Fact]
        public void CarIsUnderMaxWeightLimit()
        {
            var subjectUnderTest = new Car(logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit - 1;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
            Assert.False(subjectUnderTest.IsOverMaxWeightLimit);
        }

        [Fact]
        public void CarIsOverMaxWeightLimit()
        {
            var subjectUnderTest = new Car(logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit + 1;

            Assert.False(subjectUnderTest.IsUnderMaxWeightLimit);
            Assert.True(subjectUnderTest.IsOverMaxWeightLimit);
        }

        [Fact]
        public void CarIsAtMaxWeightLimit()
        {
            var subjectUnderTest = new Car(logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
            Assert.False(subjectUnderTest.IsOverMaxWeightLimit);
        }

        [Fact]
        public void DirectionOfTravelShouldBeUpWhenDestinationIsAboveCurrent()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = chance.Integer(1);
            subjectUnderTest.DestinationFloor = subjectUnderTest.CurrentFloor + 1;
            subjectUnderTest.SetDirectionOfTravel();
            Assert.Equal(DirectionOfTravel.UP, subjectUnderTest.DirectionOfTravel);
        }

        [Fact]
        public void DirectionOfTravelShouldBeDownWhenDestinationIsBelowCurrent()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = chance.Integer(2);
            subjectUnderTest.DestinationFloor = subjectUnderTest.CurrentFloor - 1;
            subjectUnderTest.SetDirectionOfTravel();
            Assert.Equal(DirectionOfTravel.DOWN, subjectUnderTest.DirectionOfTravel);
        }

        [Fact]
        public void DirectionOfTravelShouldBeNoneWhenDestinationTheSameAsCurrent()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = chance.Integer(1);
            subjectUnderTest.DestinationFloor = subjectUnderTest.CurrentFloor;
            subjectUnderTest.SetDirectionOfTravel();
            Assert.Equal(DirectionOfTravel.NONE, subjectUnderTest.DirectionOfTravel);
        }

        [Fact]
        public void DirectionOfTravelShouldBeNoneWhenDestinationIsZero()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = chance.Integer(1);
            subjectUnderTest.DestinationFloor = 0;
            subjectUnderTest.SetDirectionOfTravel();
            Assert.Equal(DirectionOfTravel.NONE, subjectUnderTest.DirectionOfTravel);
        }

        [Fact]
        public void NextFloorShouldBeOneUpWhenMovingUp()
        {
            var subjectUnderTest = new Car(logger);
            var currentFloor = chance.Integer(1);
            var expectedFloor = currentFloor + 1;
            subjectUnderTest.CurrentFloor = currentFloor;
            subjectUnderTest.DirectionOfTravel = DirectionOfTravel.UP;

            subjectUnderTest.SetNextFloor();

            Assert.Equal(expectedFloor, subjectUnderTest.NextFloor);
        }

        [Fact]
        public void NextFloorShouldBeOneDownWhenMovingDown()
        {
            var subjectUnderTest = new Car(logger);
            var currentFloor = chance.Integer(1);
            var expectedFloor = currentFloor - 1;
            subjectUnderTest.CurrentFloor = currentFloor;
            subjectUnderTest.DirectionOfTravel = DirectionOfTravel.DOWN;

            subjectUnderTest.SetNextFloor();

            Assert.Equal(expectedFloor, subjectUnderTest.NextFloor);
        }

        [Fact]
        public void NextFloorShouldBeCurrentFloorWhenNotMoving()
        {
            var subjectUnderTest = new Car(logger);
            var currentFloor = chance.Integer(1);
            var expectedFloor = currentFloor;
            subjectUnderTest.CurrentFloor = currentFloor;
            subjectUnderTest.DirectionOfTravel = DirectionOfTravel.NONE;

            subjectUnderTest.SetNextFloor();

            Assert.Equal(expectedFloor, subjectUnderTest.NextFloor);
        }

        [Fact]
        public void DestinationFloorShouldBe0WhenNoButtonsHaveBeenPressed()
        {
            var subjectUnderTest = new Car(logger);

            subjectUnderTest.SetDestinationFloor();

            Assert.Equal(0, subjectUnderTest.DestinationFloor);
        }

        [Fact]
        public void DestinationFloorShouldBeNextFloorWhenFloorAboveIsPressed()
        {
            var subjectUnderTest = new Car(logger);

            var buttonPressDown = new ButtonPress()
            {
                FloorNumber = chance.Integer(1),
                DirectionOfTravel = DirectionOfTravel.DOWN,
            };

            subjectUnderTest.ButtonPresses.Add(buttonPressDown);

            subjectUnderTest.SetDestinationFloor();

            Assert.Equal(buttonPressDown.FloorNumber, subjectUnderTest.DestinationFloor);
        }

        [Fact]
        public void DestinationFloorShouldBeFloorAboveWhenFloorsOnBothSidesArePickedAndGoingUp()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = 2;
            subjectUnderTest.DirectionOfTravel = DirectionOfTravel.UP;

            var buttonPress3 = new ButtonPress()
            {
                FloorNumber = 3,
                DirectionOfTravel = DirectionOfTravel.NONE,
            };
            var buttonPress1 = new ButtonPress()
            {
                FloorNumber = 1,
                DirectionOfTravel = DirectionOfTravel.NONE,
            };

            subjectUnderTest.ButtonPresses.Add(buttonPress1);
            subjectUnderTest.ButtonPresses.Add(buttonPress3);

            subjectUnderTest.SetDestinationFloor();

            Assert.Equal(buttonPress3.FloorNumber, subjectUnderTest.DestinationFloor);
        }

        [Fact]
        public void DestinationFloorShouldBeFloorBelowWhenFloorsOnBothSidesArePickedAndGoingDown()
        {
            var subjectUnderTest = new Car(logger);
            subjectUnderTest.CurrentFloor = 2;
            subjectUnderTest.DirectionOfTravel = DirectionOfTravel.DOWN;

            var buttonPress3 = new ButtonPress()
            {
                FloorNumber = 3,
                DirectionOfTravel = DirectionOfTravel.NONE,
            };
            var buttonPress1 = new ButtonPress()
            {
                FloorNumber = 1,
                DirectionOfTravel = DirectionOfTravel.NONE,
            };

            subjectUnderTest.ButtonPresses.Add(buttonPress1);
            subjectUnderTest.ButtonPresses.Add(buttonPress3);

            subjectUnderTest.SetDestinationFloor();

            Assert.Equal(buttonPress1.FloorNumber, subjectUnderTest.DestinationFloor);
        }
    }
}
