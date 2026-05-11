using AirportRCGA.Models;

namespace AirportRCGA.Core;

// F(X) = sum_i  P_i * min_k  d_H(city_i, airport_k)
public sealed class ObjectiveFunction
{
    private readonly IReadOnlyList<City> _cities;

    public ObjectiveFunction(IReadOnlyList<City> cities)
    {
        _cities = cities;
    }

    public double Evaluate(double[] genes, int numAirports)
    {
        double totalCost = 0.0;

        foreach (City city in _cities)
        {
            double minDistance = double.MaxValue;

            for (int k = 0; k < numAirports; k++)
            {
                double dist = GeoDistance.Haversine(
                    city.LatRad, city.LonRad,
                    genes[2 * k], genes[2 * k + 1]);

                if (dist < minDistance)
                    minDistance = dist;
            }

            totalCost += city.Population * minDistance;
        }

        return totalCost;
    }
}
