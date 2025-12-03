using System;
using Xunit;
using DateProject;
using Moq;
using System.Collections.Generic;
using System.IO;

namespace CustomDate.Tests
{
    public class DateTests
    {
        // -----------------------------
        // MonthName tests
        // -----------------------------
        [Theory]
        [InlineData(1, "January")]
        [InlineData(2, "February")]
        [InlineData(3, "March")]
        [InlineData(4, "April")]
        [InlineData(5, "May")]
        [InlineData(6, "August")]
        [InlineData(7, "September")]
        [InlineData(10, "October")]
        [InlineData(11, "November")]
        [InlineData(12, "December")]
        public void MonthName_ReturnsCorrectMonth(int month, string expectedName)
        {
            Date d = new Date(2020, month, 1);
            Assert.Equal(expectedName, d.MonthName);
        }

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
        public void MonthAbbrev_ReturnsCorrectAbbreviation(int month, string expectedAbbrev)
        {
            Date d = new Date(2020, month, 1);
            Assert.Equal(expectedAbbrev, d.MonthNameAbbrev);
        }

        // -----------------------------
        // Constructor validation tests
        // -----------------------------
        [Fact]
        public void Constructor_YearOutOfRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(-10000, 1, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void Constructor_MonthOutOfRange_Throws(int month)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(2020, month, 1));
        }

        [Theory]
        [InlineData(4, 31)] // 30-day month
        [InlineData(2, 29)] // Feb
        [InlineData(1, 32)] // 31-day month
        public void Constructor_InvalidDay_Throws(int month, int day)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Date(2020, month, day));
        }

        [Theory]
        [InlineData(1, 31)]
        [InlineData(3, 31)]
        [InlineData(5, 31)]
        [InlineData(7, 31)]
        [InlineData(8, 31)]
        [InlineData(10, 31)]
        [InlineData(12, 31)]
        public void Constructor_ValidDay_SetsDateCorrectly(int month, int day)
        {
            Date d = new Date(2020, month, day);
            Assert.Equal(2020, d.Year);
            Assert.Equal(month, d.Month);
            Assert.Equal(day, d.Day);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(2, 28)]
        public void Constructor_FebruaryValidDays_SetsCorrectly(int month, int day)
        {
            Date d = new Date(2020, month, day);
            Assert.Equal(2020, d.Year);
            Assert.Equal(month, d.Month);
            Assert.Equal(day, d.Day);
        }

        // -----------------------------
        // AddOneMonth tests
        // -----------------------------
        [Theory]
        [InlineData(2020, 1, 15, 2020, 2, 15)]
        [InlineData(2020, 1, 31, 2020, 2, 28)]
        [InlineData(2020, 4, 15, 2020, 5, 15)]
        [InlineData(2020, 7, 31, 2020, 8, 31)]
        [InlineData(2020, 12, 10, 2021, 1, 10)]
        [InlineData(2020, 12, 31, 2021, 1, 31)]
        public void AddOneMonth_ReturnsCorrectDate(int year, int month, int day,
                                                   int expectedYear, int expectedMonth, int expectedDay)
        {
            Date d = new Date(year, month, day);
            Date result = d.AddOneMonth();
            Assert.Equal(expectedYear, result.Year);
            Assert.Equal(expectedMonth, result.Month);
            Assert.Equal(expectedDay, result.Day);
        }

        // -----------------------------
        // IsToday tests with mock
        // -----------------------------
        [Fact]
        public void IsToday_MatchingDate_ReturnsTrue()
        {
            var mockProvider = new Mock<ISystemDateProvider>();
            mockProvider.Setup(p => p.GetToday()).Returns(new Date(2020, 1, 1));

            var d = new Date(2020, 1, 1, mockProvider.Object);
            Assert.True(d.IsToday());
        }

        [Fact]
        public void IsToday_NonMatchingDate_ReturnsFalse()
        {
            var mockProvider = new Mock<ISystemDateProvider>();
            mockProvider.Setup(p => p.GetToday()).Returns(new Date(2020, 1, 1));

            var d = new Date(2020, 1, 2, mockProvider.Object);
            Assert.False(d.IsToday());
        }

        // -----------------------------
        // WhatHolidayIsOnThisDay tests
        // -----------------------------
        [Fact]
        public void WhatHolidayIsOnThisDay_SingleHoliday_ReturnsName()
        {
            var mock = new Mock<IHolidayProvider>();
            var holidays = new List<Holiday>
            {
                new Holiday { TheDate = new Date(2020, 10, 31), Name = "Halloween" },
                new Holiday { TheDate = new Date(2020, 12, 25), Name = "Christmas" }
            };
            mock.Setup(h => h.GetHolidays(2020)).Returns(holidays);

            var d = new Date(2020, 10, 31, mock.Object);
            var result = d.WhatHolidayIsOnThisDay();
            Assert.Equal("Halloween", result);
        }

        [Fact]
        public void WhatHolidayIsOnThisDay_NoHoliday_ReturnsNull()
        {
            var mock = new Mock<IHolidayProvider>();
            var holidays = new List<Holiday>
            {
                new Holiday { TheDate = new Date(2020, 12, 25), Name = "Christmas" },
                new Holiday { TheDate = new Date(2020, 10, 31), Name = "Halloween" }
            };
            mock.Setup(h => h.GetHolidays(2020)).Returns(holidays);

            var d = new Date(2020, 7, 4, mock.Object);
            var result = d.WhatHolidayIsOnThisDay();
            Assert.Null(result);
        }

        // -----------------------------
        // WhatHolidaysAreOnThisDay tests
        // -----------------------------
        [Fact]
        public void WhatHolidaysAreOnThisDay_OneHoliday_ReturnsSingle()
        {
            var mock = new Mock<IHolidayProvider>();
            var holidays = new List<Holiday>
            {
                new Holiday { TheDate = new Date(2020, 10, 31), Name = "Halloween" }
            };
            mock.Setup(h => h.GetHolidays(2020)).Returns(holidays);

            var d = new Date(2020, 10, 31, mock.Object);
            var result = d.WhatHolidaysAreOnThisDay();

            Assert.Single(result);
            Assert.Contains("Halloween", result);
        }

        [Fact]
        public void WhatHolidaysAreOnThisDay_MultipleHolidays_ReturnsAll()
        {
            var mock = new Mock<IHolidayProvider>();
            var holidays = new List<Holiday>
            {
                new Holiday { TheDate = new Date(2020, 12, 25), Name = "Christmas" },
                new Holiday { TheDate = new Date(2020, 12, 25), Name = "Gift Day" }
            };
            mock.Setup(h => h.GetHolidays(2020)).Returns(holidays);

            var d = new Date(2020, 12, 25, mock.Object);
            var result = d.WhatHolidaysAreOnThisDay();

            Assert.Equal(2, result.Count);
            Assert.Contains("Christmas", result);
            Assert.Contains("Gift Day", result);
        }

        [Fact]
        public void WhatHolidaysAreOnThisDay_NoMatches_ReturnsEmpty()
        {
            var mock = new Mock<IHolidayProvider>();
            var holidays = new List<Holiday>
            {
                new Holiday { TheDate = new Date(2020, 12, 25), Name = "Christmas" }
            };
            mock.Setup(h => h.GetHolidays(2020)).Returns(holidays);

            var d = new Date(2020, 7, 4, mock.Object);
            var result = d.WhatHolidaysAreOnThisDay();
            Assert.Empty(result);
        }

        [Fact]
        public void WhatHolidaysAreOnThisDay_EmptyList_ReturnsEmpty()
        {
            var mock = new Mock<IHolidayProvider>();
            mock.Setup(h => h.GetHolidays(2020)).Returns(new List<Holiday>());

            var d = new Date(2020, 1, 1, mock.Object);
            var result = d.WhatHolidaysAreOnThisDay();
            Assert.Empty(result);
        }

        [Fact]
        public void WhatHolidaysAreOnThisDay_NullList_ReturnsEmpty()
        {
            var mock = new Mock<IHolidayProvider>();
            mock.Setup(h => h.GetHolidays(2020)).Returns((List<Holiday>)null);

            var d = new Date(2020, 1, 1, mock.Object);
            var result = d.WhatHolidaysAreOnThisDay();
            Assert.Empty(result);
        }

        // -----------------------------
        // WhoseBirthdayIsIt tests
        // -----------------------------

        private string CreateTempBirthdayFile(params string[] lines)
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, lines);
            return tempFile;
        }

        [Fact]
        public void WhoseBirthdayIsIt_ReturnsMatchingNames()
        {
            var date = new Date(2025, 12, 3);
            string filePath = CreateTempBirthdayFile(
                "John,1990-12-03",
                "Mary,1985-07-04",
                "Alice,2000-12-03"
            );

            var result = date.WhoseBirthdayIsIt(filePath);

            Assert.Contains("John", result);
            Assert.Contains("Alice", result);
            Assert.DoesNotContain("Mary", result);

            File.Delete(filePath);
        }

        [Fact]
        public void WhoseBirthdayIsIt_ReturnsEmptyIfNoMatch()
        {
            var date = new Date(2025, 1, 1);
            string filePath = CreateTempBirthdayFile(
                "John,1990-12-03",
                "Mary,1985-07-04"
            );

            var result = date.WhoseBirthdayIsIt(filePath);

            Assert.Empty(result);

            File.Delete(filePath);
        }

        [Fact]
        public void WhoseBirthdayIsIt_ReturnsEmptyIfFileDoesNotExist()
        {
            var date = new Date(2025, 1, 1);
            string filePath = "nonexistentfile.csv";

            var result = date.WhoseBirthdayIsIt(filePath);

            Assert.Empty(result);
        }

        [Fact]
        public void WhoseBirthdayIsIt_IgnoresInvalidLines()
        {
            var date = new Date(2025, 12, 3);
            string filePath = CreateTempBirthdayFile(
                "John,1990-12-03",
                "InvalidLineWithoutComma",
                "Alice,NotADate",
                "Bob,2025-12-03"
            );

            var result = date.WhoseBirthdayIsIt(filePath);

            Assert.Contains("John", result);
            Assert.Contains("Bob", result);
            Assert.DoesNotContain("Alice", result);

            File.Delete(filePath);
        }
    }
}
