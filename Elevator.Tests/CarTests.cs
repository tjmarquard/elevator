namespace Elevator.Tests
{
    using Serilog;
    using Xunit;

    public class CarTests
    {
        private readonly ILogger logger;

        public CarTests()
        {
            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/elevator.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        [Fact]
        public void ElevatorShouldStartWithAnEmptyFloorQueue()
        {
            var subjectUnderTest = new Car(5, logger);

            Assert.Empty(subjectUnderTest.ButtonPresses);
        }

        [Fact]
        public void CarShouldStartStaionary()
        {
            var subjectUnderTest = new Car(5, logger);

            Assert.Equal(DirectionOfTravel.NONE, subjectUnderTest.DirectionOfTravel);
            Assert.False(subjectUnderTest.IsMoving);
        }

        [Fact]
        public void CarShouldHaveFiveFloors()
        {
            var subjectUnderTest = new Car(5, logger);

            Assert.Collection<int>(
                subjectUnderTest.Floors,
                floor => Assert.Equal(1, floor),
                floor => Assert.Equal(2, floor),
                floor => Assert.Equal(3, floor),
                floor => Assert.Equal(4, floor),
                floor => Assert.Equal(5, floor));
        }

        [Fact]
        public void CarShouldStartOnFirstFloor()
        {
            var subjectUnderTest = new Car(5, logger);

            Assert.Equal(1, subjectUnderTest.CurrentFloor);
        }

        [Fact]
        public void CarHasAMaxWeightLimitOf2000()
        {
            var subjectUnderTest = new Car(5, logger);

            Assert.Equal(2000, subjectUnderTest.MaxWeightLimit);
        }

        [Fact]
        public void CarIsUnderMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5, logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit - 1;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
        }

        [Fact]
        public void CarIsOverMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5, logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit + 1;

            Assert.False(subjectUnderTest.IsUnderMaxWeightLimit);
        }

        [Fact]
        public void CarIsAtMaxWeightLimit()
        {
            var subjectUnderTest = new Car(5, logger);

            subjectUnderTest.Weight = subjectUnderTest.MaxWeightLimit;

            Assert.True(subjectUnderTest.IsUnderMaxWeightLimit);
        }
    }
}
