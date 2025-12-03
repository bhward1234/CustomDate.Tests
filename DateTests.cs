using System;
using Xunit;
using DateProject;

namespace CustomDate.Tests
{
    public class DateTests
    {
        [Fact]
        public void MonthName_MonthIs1_ReturnsJanuary()
        {
            //Arrange
            Date d = new Date();

            //Act
            string monthName = d.MonthName;

            //Assert
            Assert.Equal("January", monthName);

        }
        [Fact]
        public void MonthName_MonthIs2_ReturnsFebruary()
        {
            //Arrange
            Date d = new Date(2020, 2, 1);

            //Act
            string monthName = d.MonthName;

            //Assert
            Assert.Equal("February", monthName);

        }
        [Theory]
        [InlineData(12, "December")]
        [InlineData(11, "November")]
        [InlineData(10, "October")]
        [InlineData(7, "September")]
        [InlineData(6, "August")]
        public void MonthName_MonthIs1To12_ReturnsCorrectMonth(int monthNum, string name)
        {
            //Arrange
            Date d = new Date(2020, monthNum, 1);

            //Act
            string monthName = d.MonthName;

            //Assert
            Assert.Equal(name, monthName);

        }

        [Fact]
        public void DateConstructor_YearIsTooBig_ThrowsArgumentOutOfRange()
        {
            //Arrange
            int year = -10000;

            //Act
            Date d;// = new Date(year, 1, 1);

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(year, 1, 1));
        }

        [Fact]
        public void DateConstructor_MonthTooSmall_ThrowArgumentOutOfRangeException()
        {
            //Arrange
            int month = 0;

            //Act
            Date d;// = new Date(year, 1, 1);

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(2020, month, 1));
        }

        [Fact]
        public void DateConstructor_MonthTooBig_ThrowArgumentOutOfRangeException()
        {
            //Arrange
            int month = 13;

            //Act
            Date d;// = new Date(year, 1, 1);

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(2020, month, 1));
        }

        [Fact]
        public void DateConstructor_Monthis30DayMax_DayIsBetween1And30_DateSetAccordingToParameters()
        {
            //Arrange
            int day = 30;

            int month = 4;

            //Act
            Date d = new Date(2020, month, day);

            //Assert
            Assert.Equal(2020, d.Year);
            Assert.Equal(month, d.Month);
            Assert.Equal(day, d.Day);
        }
        [Fact]
        public void DateConstructor_Monthis30DayMax_DayIsNotBetween1And30_ThrowsArgumentInvalidException()
        {
            //Arrange
            int day = 31;

            int month = 4;

            //Act
            Date d;

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(2020, month, day));
        }

        //Add tests for Monthis31DayMax nad DayBetween1And31
        [Theory]
        [InlineData(1, 31)]   // January
        [InlineData(3, 31)]   // March
        [InlineData(5, 31)]   // May
        [InlineData(7, 31)]   // July
        [InlineData(8, 31)]   // August
        [InlineData(10, 31)]  // October
        [InlineData(12, 31)]  // December
        public void DateConstructor_MonthIs31DayMax_DayBetween1And31_DateSetCorrectly(int month, int day)
        {
            // Act
            Date d = new Date(2020, month, day);

            // Assert
            Assert.Equal(2020, d.Year);
            Assert.Equal(month, d.Month);
            Assert.Equal(day, d.Day);
        }


        //Add tests for Monthis31DayMax nad DayNotBetween1And31
        [Theory]
        [InlineData(1, 32)]
        [InlineData(3, 0)]
        [InlineData(5, 40)]
        [InlineData(7, -1)]
        public void DateConstructor_MonthIs31DayMax_DayNotBetween1And31_ThrowsArgumentOutOfRange(int month, int day)
        {
            Date d;
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(2020, month, day));
        }


        //Add tests for MonthIsFebruary and DayBetween1And28
        [Theory]
        [InlineData(1)]
        [InlineData(15)]
        [InlineData(28)]
        public void DateConstructor_MonthIsFebruary_DayBetween1And28_DateSetCorrectly(int day)
        {
            // Act
            Date d = new Date(2020, 2, day);

            // Assert
            Assert.Equal(2020, d.Year);
            Assert.Equal(2, d.Month);
            Assert.Equal(day, d.Day);
        }

        //Add tests for MonthIsFebruary and DayNotBetween1And28
        [Theory]
        [InlineData(0)]
        [InlineData(29)]
        [InlineData(35)]
        public void DateConstructor_MonthIsFebruary_DayNotBetween1And28_ThrowsArgumentOutOfRangeException(int day)
        {
            Date d;
            Assert.Throws<ArgumentOutOfRangeException>(() => d = new Date(2020, 2, day));
        }

        // Month Abbreviated
        [Theory]
        [InlineData(1, "Jan")]
        [InlineData(2, "Feb")]
        [InlineData(3, "Mar")]
        [InlineData(4, "Apr")]
        [InlineData(5, "May")]
        [InlineData(6, "Aug")]
        [InlineData(7, "Sep")]
        [InlineData(10, "Oct")]
        [InlineData(11, "Nov")]
        [InlineData(12, "Dec")]
        [InlineData(8, "Unknown")]
        [InlineData(9, "Unknown")]
        public void MonthAbbrev_ReturnsCorrectAbbreviation(int monthNum, string expectedAbbrev)
        {
            // Arrange
            Date d = new Date(2020, monthNum, 1);

            // Act
            string abbrev = d.MonthNameAbbrev;

            // Assert
            Assert.Equal(expectedAbbrev, abbrev);
        }

        // Add One Month
        public class DateAddOneMonthTests
        {
            [Theory]
            [InlineData(2020, 1, 15, 2020, 2, 15)]   // Normal month increment
            [InlineData(2020, 1, 31, 2020, 2, 28)]   // Jan 31 → Feb 28
            [InlineData(2020, 4, 15, 2020, 5, 15)]   // 30-day → 31-day month
            [InlineData(2020, 7, 31, 2020, 8, 31)]   // July 31 → Aug 31 (instructor mapping: August has 31)
            [InlineData(2020, 12, 10, 2021, 1, 10)]  // Year rollover
            [InlineData(2020, 12, 31, 2021, 1, 31)]  // Year rollover end-of-month
            public void AddOneMonth_ReturnsCorrectNextMonthDate(int year, int month, int day,
                                                           int expectedYear, int expectedMonth, int expectedDay)
            {
                // Arrange
                Date d = new Date(year, month, day);

                // Act
                Date result = d.AddOneMonth();

                // Assert
                Assert.Equal(expectedYear, result.Year);
                Assert.Equal(expectedMonth, result.Month);
                Assert.Equal(expectedDay, result.Day);
            }
        }
    }
}
