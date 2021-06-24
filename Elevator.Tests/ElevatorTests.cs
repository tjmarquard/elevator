﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Elevator.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void ElevatorShouldHaveFiveFloors()
        {
            var subjectUnderTest = new Elevator(5);

            Assert.Collection<Floor>(subjectUnderTest.Floors,
                floor => Assert.Equal(1, floor.Number),
                floor => Assert.Equal(2, floor.Number),
                floor => Assert.Equal(3, floor.Number),
                floor => Assert.Equal(4, floor.Number),
                floor => Assert.Equal(5, floor.Number)
            );

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
            var subjectUnderTest = new Elevator(5);
            var actualFloorNumber = subjectUnderTest.GetEnteredFloorNumber(enteredButton);

            Assert.Equal(expectedFloorNumber, actualFloorNumber);
        }        

        [Theory]
        [InlineData("D", 0)]
        public void GetEnteredFloorNumberShouldBeInvalid(string enteredButton, int expectedFloorNumber)
        {
            var subjectUnderTest = new Elevator(5);
            var actualFloorNumber = subjectUnderTest.GetEnteredFloorNumber(enteredButton);

            Assert.Equal(expectedFloorNumber, actualFloorNumber);
        }       
        
        [Theory]
        [InlineData("5D", "D")]
        [InlineData("4U", "U")]
        [InlineData("Q", "Q")]
        [InlineData("2B", "B")]
        public void EnteredDirectionOfTravelShouldBeValid(string enteredButton, string expectedDirection)
        {
            var subjectUnderTest = new Elevator(5);
            var actualDirection = subjectUnderTest.GetEnteredDirectionOfTravel(enteredButton);

            Assert.Equal(expectedDirection, actualDirection);
        }
    }
}