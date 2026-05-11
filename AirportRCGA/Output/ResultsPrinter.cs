using AirportRCGA.Core;

namespace AirportRCGA.Output;

public static class ResultsPrinter
{
    private const string ResultsDirectory = "results";
    private const double RadToDeg = 180.0 / Math.PI;

    public static void PrintConvergenceUpdate(int generation, double bestFitness)
    {
        if (generation % GaParameters.ConvergencePrintInterval == 0)
            Console.WriteLine($"  Gen {generation,4}: Best F = {bestFitness:N0}");
    }

    public static void PrintFinalResult(Individual best, int numAirports)
    {
        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine($"  │  Optimised Airport Locations  (N = {numAirports})                  │");
        Console.WriteLine("  ├──────────┬──────────────────────┬──────────────────────┤");
        Console.WriteLine("  │ Airport  │  Latitude            │  Longitude           │");
        Console.WriteLine("  ├──────────┼──────────────────────┼──────────────────────┤");

        for (int k = 0; k < numAirports; k++)
        {
            double latRad = best.Genes[2 * k];
            double lonRad = best.Genes[2 * k + 1];
            Console.WriteLine(
                $"  │  {k + 1,-7} │  {latRad:F6} rad ({latRad * RadToDeg:F4}°N)  │  {lonRad:F6} rad ({lonRad * RadToDeg:F4}°E)  │");
        }

        Console.WriteLine("  └──────────┴──────────────────────┴──────────────────────┘");
        Console.WriteLine($"  Objective F = {best.Fitness:N0} person·km");
    }

    public static void PrintMode2Table(
        IReadOnlyList<(int Seed, Individual Best)> results,
        int numAirports)
    {
        Console.WriteLine();
        Console.WriteLine($"  Results for N = {numAirports} across {results.Count} seeds");
        Console.WriteLine();

        string header = $"  {"Seed",-6} | {"Best F",20}";
        for (int k = 0; k < numAirports; k++)
            header += $" | Apt{k + 1} Lat(°) | Apt{k + 1} Lon(°)";
        Console.WriteLine(header);
        Console.WriteLine(new string('-', header.Length + 4));

        foreach (var (seed, best) in results)
        {
            string row = $"  {seed,-6} | {best.Fitness,20:N0}";
            for (int k = 0; k < numAirports; k++)
            {
                row += $" | {best.Genes[2 * k] * RadToDeg,10:F4}  | {best.Genes[2 * k + 1] * RadToDeg,10:F4} ";
            }
            Console.WriteLine(row);
        }

        double[] fitnesses = results.Select(r => r.Best.Fitness).ToArray();
        double mean   = fitnesses.Average();
        double stdDev = Math.Sqrt(fitnesses.Average(f => (f - mean) * (f - mean)));

        Console.WriteLine();
        Console.WriteLine($"  Mean F  = {mean:N0}");
        Console.WriteLine($"  Std Dev = {stdDev:N0}");
        Console.WriteLine($"  Min F   = {fitnesses.Min():N0}");
        Console.WriteLine($"  Max F   = {fitnesses.Max():N0}");
    }

    public static void PrintMode3SummaryTable(
        IReadOnlyList<(int N, double MeanF, double StdDev, double MinF, double MaxF)> rows)
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════╦══════════════════╦══════════════╦══════════════════╦══════════════════╗");
        Console.WriteLine("  ║  N   ║ Mean F           ║ Std Dev      ║ Reduction vs N=1 ║ Marginal vs N-1  ║");
        Console.WriteLine("  ╠══════╬══════════════════╬══════════════╬══════════════════╬══════════════════╣");

        double baselineMean = rows.FirstOrDefault(r => r.N == 1).MeanF;
        double? prevMean = null;

        foreach (var (n, mean, std, _, _) in rows)
        {
            string reductionVsOne = (n == 1 || baselineMean <= 0)
                ? "  N/A              "
                : $"  {(baselineMean - mean) / baselineMean * 100.0,8:F2} %        ";

            string marginalGain = (prevMean == null || prevMean.Value <= 0)
                ? "  N/A              "
                : $"  {(prevMean.Value - mean) / prevMean.Value * 100.0,8:F2} %        ";

            Console.WriteLine(
                $"  ║ {n,4} ║ {mean,16:N0} ║ {std,12:N0} ║{reductionVsOne}║{marginalGain}║");

            prevMean = mean;
        }

        Console.WriteLine("  ╚══════╩══════════════════╩══════════════╩══════════════════╩══════════════════╝");
    }

    public static void PrintFullCoordinateTable(Individual best, int numAirports, int seed)
    {
        Console.WriteLine();
        Console.WriteLine($"  Full airport coordinates — N = {numAirports}, seed = {seed}");
        Console.WriteLine($"  F = {best.Fitness:N0} person·km");
        Console.WriteLine();
        Console.WriteLine($"  {"Airport",-10} {"Lat (rad)",12} {"Lon (rad)",12} {"Lat (°N)",10} {"Lon (°E)",10}");
        Console.WriteLine(new string('-', 60));

        for (int k = 0; k < numAirports; k++)
        {
            double latRad = best.Genes[2 * k];
            double lonRad = best.Genes[2 * k + 1];
            Console.WriteLine(
                $"  {k + 1,-10} {latRad,12:F6} {lonRad,12:F6} {latRad * RadToDeg,10:F4} {lonRad * RadToDeg,10:F4}");
        }
    }

    public static void WriteConvergenceCsv(
        int numAirports,
        int seed,
        IReadOnlyList<(int Generation, double BestFitness)> log)
    {
        Directory.CreateDirectory(ResultsDirectory);
        string path = Path.Combine(ResultsDirectory, $"convergence_N{numAirports}_seed{seed}.csv");

        using var writer = new StreamWriter(path);
        writer.WriteLine("Generation,BestFitness");
        foreach (var (generation, bestFitness) in log)
            writer.WriteLine($"{generation},{bestFitness:F4}");

        Console.WriteLine($"  CSV written → {path}");
    }
}
