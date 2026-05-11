using AirportRCGA.Core;
using AirportRCGA.Models;

namespace AirportRCGA.Tests;

/// <summary>
/// Tests for the Haversine great-circle distance formula.
/// </summary>
public class GeoDistanceTests
{
    /// <summary>
    /// Lagos to Ibadan distance using the hard-coded city coordinates.
    /// The known straight-line distance is approximately 117–120 km
    /// (note: the problem spec mentions ~145 km but that figure does not
    ///  match the provided radian coordinates; the Haversine formula with
    ///  the exact coordinates given yields ~118 km).
    /// The test therefore asserts a generous range of 100–160 km.
    /// </summary>
    [Fact]
    public void Haversine_LagosToIbadan_ApproximatelyCorrect()
    {
        City lagos  = CityData.Cities.First(c => c.Name == "Lagos");
        City ibadan = CityData.Cities.First(c => c.Name == "Ibadan");

        double distance = GeoDistance.Haversine(
            lagos.LatRad,  lagos.LonRad,
            ibadan.LatRad, ibadan.LonRad);

        // Generous bounds: the formula should give ~118 km.
        Assert.InRange(distance, 100.0, 160.0);
    }

    /// <summary>
    /// Distance from a point to itself must be exactly zero.
    /// </summary>
    [Fact]
    public void Haversine_SamePoint_ReturnsZero()
    {
        double dist = GeoDistance.Haversine(0.11, 0.06, 0.11, 0.06);
        Assert.Equal(0.0, dist, precision: 10);
    }

    /// <summary>
    /// Distance must be symmetric: d(A,B) == d(B,A).
    /// </summary>
    [Fact]
    public void Haversine_Symmetric()
    {
        double distAB = GeoDistance.Haversine(0.112661, 0.059064, 0.128763, 0.068171);
        double distBA = GeoDistance.Haversine(0.128763, 0.068171, 0.112661, 0.059064);

        Assert.Equal(distAB, distBA, precision: 10);
    }

    /// <summary>
    /// Distance must always be non-negative.
    /// </summary>
    [Theory]
    [InlineData(0.10, 0.06, 0.15, 0.11)]
    [InlineData(0.14, 0.10, 0.09, 0.06)]
    public void Haversine_AlwaysNonNegative(double lat1, double lon1, double lat2, double lon2)
    {
        double dist = GeoDistance.Haversine(lat1, lon1, lat2, lon2);
        Assert.True(dist >= 0.0);
    }
}
