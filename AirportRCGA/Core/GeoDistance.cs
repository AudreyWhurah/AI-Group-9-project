namespace AirportRCGA.Core;

public static class GeoDistance
{
    public static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double deltaLat = lat2 - lat1;
        double deltaLon = lon2 - lon1;

        double sinHalfLat = Math.Sin(deltaLat * 0.5);
        double sinHalfLon = Math.Sin(deltaLon * 0.5);

        double a = sinHalfLat * sinHalfLat
                 + Math.Cos(lat1) * Math.Cos(lat2) * sinHalfLon * sinHalfLon;

        // Clamp to [0,1] — floating-point rounding can push a slightly above 1
        double c = 2.0 * Math.Asin(Math.Sqrt(Math.Min(1.0, a)));

        return GaParameters.EarthRadiusKm * c;
    }
}
