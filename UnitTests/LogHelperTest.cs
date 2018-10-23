using System;
using Xunit;
using AttendanceWebApi.Helpers;

namespace UnitTests
{
    public class LogHelperTest
    {
        [Fact]
        public void Test1()
        {
            // UOM
            var lat1 = 40.625376;
            var lon1 = 22.960046;

            // Home
            var lat2 = 40.596757;
            var lon2 = 22.986897;

            // Calculated distance
            var actual = LogHelper.DistanceInMeters(lat1, lon1, lat2, lon2);

            Assert.True(actual > 0);
        }
    }
}
