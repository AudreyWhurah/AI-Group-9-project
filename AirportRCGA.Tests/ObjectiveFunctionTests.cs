using AirportRCGA.Core;
using AirportRCGA.Models;

namespace AirportRCGA.Tests;

/// <summary>
/// Tests for the airport facility-location objective function F(X).
/// </summary>
public class ObjectiveFunctionTests
{
    private readonly ObjectiveFunction _objFunc = new(CityData.Cities);

    /// <summary>
    /// When N = 20 airports are placed exactly at the 20 city coordinates
    /// every city is at distance 0 from its nearest airport, so F = 0.
    /// </summary>
    [Fact]
    public void Evaluate_AllCitiesOnAirport_ReturnsZero()
    {
        var cities = CityData.Cities;
        int n      = cities.Length;

        // Build chromosome: airport k sits at city k.
        var genes = new double[2 * n];
        for (int k = 0; k < n; k++)
        {
            genes[2 * k]     = cities[k].LatRad;
            genes[2 * k + 1] = cities[k].LonRad;
        }

        double result = _objFunc.Evaluate(genes, n);

        // Distance = 0 for every city → F = 0.
        Assert.Equal(0.0, result, precision: 6);
    }

    /// <summary>
    /// F(X) must always be non-negative because it is a sum of non-negative terms.
    /// </summary>
    [Fact]
    public void Evaluate_AlwaysNonNegative()
    {
        var rng   = new Random(123);
        var objFn = new ObjectiveFunction(CityData.Cities);

        for (int trial = 0; trial < 100; trial++)
        {
            int n     = rng.Next(1, 9);
            var genes = new double[2 * n];
            for (int j = 0; j < genes.Length; j += 2)
            {
                genes[j]     = GaParameters.LatMin + rng.NextDouble() * (GaParameters.LatMax - GaParameters.LatMin);
                genes[j + 1] = GaParameters.LonMin + rng.NextDouble() * (GaParameters.LonMax - GaParameters.LonMin);
            }

            double f = objFn.Evaluate(genes, n);
            Assert.True(f >= 0.0, $"F was negative: {f}");
        }
    }

    /// <summary>
    /// Adding more airports (N2 &gt; N1) should never increase F, because an
    /// additional airport can only reduce or maintain the minimum distance for each city.
    /// We verify this for one specific random chromosome (airport positions are the same
    /// for the first N1 airports, with the extra one added centrally).
    /// </summary>
    [Fact]
    public void Evaluate_MoreAirportsCannotIncreaseF()
    {
        // Chromosome for N=3.
        var genes3 = new double[]
        {
            0.112661, 0.059064,  // airport 0 at Lagos
            0.128763, 0.068171,  // airport 1 at Ibadan
            0.110567, 0.098210,  // airport 2 at Benin City
        };

        // Add a fourth airport at the centroid — cannot make things worse.
        var genes4 = new double[]
        {
            0.112661, 0.059064,
            0.128763, 0.068171,
            0.110567, 0.098210,
            0.125000, 0.085000,  // additional airport in the middle
        };

        double f3 = _objFunc.Evaluate(genes3, 3);
        double f4 = _objFunc.Evaluate(genes4, 4);

        Assert.True(f4 <= f3 + 1.0, // small tolerance for floating-point
            $"F with 4 airports ({f4:N0}) was greater than with 3 ({f3:N0})");
    }
}
