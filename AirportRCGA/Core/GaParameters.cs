namespace AirportRCGA.Core;

public static class GaParameters
{
    public const int PopulationSize = 100;
    public const int MaxGenerations = 500;
    public const double CrossoverProbability = 0.90;
    public const double EarthRadiusKm = 6371.0;

    // Bounding box for airport coordinates — all values in radians
    public const double LatMin = 0.09599;
    public const double LatMax = 0.15708;
    public const double LonMin = 0.05760;
    public const double LonMax = 0.11868;

    // If f_max - f_min falls below this, treat all fitnesses as equal (FUSS)
    public const double FussEqualityThreshold = 1e-15;

    public const int ConvergencePrintInterval = 50;

    public static readonly int[] ExperimentSeeds = { 42, 123, 456, 789, 1024 };
    public static readonly int[] SweepNValues = { 1, 3, 4, 5, 6, 7, 8 };

    // P_m = 1/(2N) gives an expected one mutation per chromosome
    public static double MutationRate(int numAirports) => 1.0 / (2.0 * numAirports);
}
