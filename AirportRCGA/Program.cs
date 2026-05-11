using AirportRCGA.Core;
using AirportRCGA.GA;
using AirportRCGA.Models;
using AirportRCGA.Operators;
using AirportRCGA.Output;

// ── Shared objective-function instance (city data never changes) ─────────────
var objectiveFunction = new ObjectiveFunction(CityData.Cities);

// ── Main menu loop ────────────────────────────────────────────────────────────
while (true)
{
    PrintMenu();
    string choice = Console.ReadLine()?.Trim() ?? string.Empty;

    switch (choice)
    {
        case "1": RunMode1(); break;
        case "2": RunMode2(); break;
        case "3": RunMode3(); break;
        case "4": Console.WriteLine("Goodbye."); return;
        default:  Console.WriteLine("  Invalid choice — please enter 1, 2, 3, or 4."); break;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Helper: print the main menu
// ─────────────────────────────────────────────────────────────────────────────
static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("  ┌─────────────────────────────────────────┐");
    Console.WriteLine("  │   Group 9 (G09) Airport RCGA            │");
    Console.WriteLine("  │   Flat Crossover | UPM | FUSS           │");
    Console.WriteLine("  ├─────────────────────────────────────────┤");
    Console.WriteLine("  │  [1] Run single N (one seed)            │");
    Console.WriteLine("  │  [2] Run single N (5 seeds, statistics) │");
    Console.WriteLine("  │  [3] Sweep N = {1,3..8} (full expt.)   │");
    Console.WriteLine("  │  [4] Exit                               │");
    Console.WriteLine("  └─────────────────────────────────────────┘");
    Console.Write("  Choice: ");
}

// ─────────────────────────────────────────────────────────────────────────────
// Mode 1 — single N, single seed
// ─────────────────────────────────────────────────────────────────────────────
void RunMode1()
{
    int n    = PromptInt("  Enter number of airports (N): ", 1, 20);
    int seed = PromptInt("  Enter random seed: ", int.MinValue, int.MaxValue);

    Console.WriteLine();
    Console.WriteLine($"  Running N={n}, seed={seed} …");

    var (best, _) = RunGA(n, seed, printConvergence: true, writeCsv: true);

    ResultsPrinter.PrintFinalResult(best, n);
}

// ─────────────────────────────────────────────────────────────────────────────
// Mode 2 — single N, five seeds, statistics
// ─────────────────────────────────────────────────────────────────────────────
void RunMode2()
{
    int n = PromptInt("  Enter number of airports (N): ", 1, 20);

    Console.WriteLine();
    Console.WriteLine($"  Running N={n} with seeds {string.Join(", ", GaParameters.ExperimentSeeds)} …");

    var results = new List<(int Seed, Individual Best)>();

    foreach (int seed in GaParameters.ExperimentSeeds)
    {
        Console.Write($"    seed {seed} … ");
        var (best, _) = RunGA(n, seed, printConvergence: false, writeCsv: false);
        Console.WriteLine($"F = {best.Fitness:N0}");
        results.Add((seed, best));
    }

    ResultsPrinter.PrintMode2Table(results, n);
}

// ─────────────────────────────────────────────────────────────────────────────
// Mode 3 — full sweep N ∈ {1,3,4,5,6,7,8}, five seeds each
// ─────────────────────────────────────────────────────────────────────────────
void RunMode3()
{
    Console.WriteLine();
    Console.WriteLine("  Full sweep: N ∈ {1,3,4,5,6,7,8}, seeds {42,123,456,789,1024}");
    Console.WriteLine("  (Writing CSV convergence files to results/ …)");
    Console.WriteLine();

    var summaryRows = new List<(int N, double MeanF, double StdDev, double MinF, double MaxF)>();

    // Store seed-42 results for N=4 and N=6 to print coordinate tables later.
    Individual? bestN4Seed42 = null;
    Individual? bestN6Seed42 = null;

    foreach (int n in GaParameters.SweepNValues)
    {
        var fitnesses = new List<double>();

        foreach (int seed in GaParameters.ExperimentSeeds)
        {
            Console.Write($"  N={n}, seed={seed} … ");
            var (best, _) = RunGA(n, seed, printConvergence: false, writeCsv: true);
            Console.WriteLine($"F = {best.Fitness:N0}");
            fitnesses.Add(best.Fitness);

            if (n == 4 && seed == 42) bestN4Seed42 = best;
            if (n == 6 && seed == 42) bestN6Seed42 = best;
        }

        double mean = fitnesses.Average();
        double std  = Math.Sqrt(fitnesses.Average(f => (f - mean) * (f - mean)));
        double minF = fitnesses.Min();
        double maxF = fitnesses.Max();

        summaryRows.Add((n, mean, std, minF, maxF));
        Console.WriteLine($"  N={n}: Mean={mean:N0}, Std={std:N0}");
        Console.WriteLine();
    }

    // Summary table for report.
    Console.WriteLine();
    Console.WriteLine("  ══════════════════════  SUMMARY TABLE  ══════════════════════");
    ResultsPrinter.PrintMode3SummaryTable(summaryRows);

    // Detailed coordinate tables for N=4 and N=6 (seed 42).
    if (bestN4Seed42 is not null)
        ResultsPrinter.PrintFullCoordinateTable(bestN4Seed42, 4, 42);
    if (bestN6Seed42 is not null)
        ResultsPrinter.PrintFullCoordinateTable(bestN6Seed42, 6, 42);
}

// ─────────────────────────────────────────────────────────────────────────────
// Core helper: construct operators and run the GA for one (N, seed) pair
// ─────────────────────────────────────────────────────────────────────────────
(Individual Best, List<(int Generation, double BestFitness)> Log) RunGA(
    int numAirports, int seed, bool printConvergence, bool writeCsv)
{
    var rng       = new Random(seed);
    var crossover = new FlatCrossover();
    var mutation  = new UniformPerturbationMutation(numAirports);
    var selection = new FussSelection();
    var engine    = new RcgaEngine(crossover, mutation, selection, objectiveFunction, rng);

    Action<int, double>? callback = printConvergence ? ResultsPrinter.PrintConvergenceUpdate : null;

    var (best, log) = engine.Run(numAirports, callback);

    if (writeCsv)
        ResultsPrinter.WriteConvergenceCsv(numAirports, seed, log);

    return (best, log);
}

// ─────────────────────────────────────────────────────────────────────────────
// Input helper
// ─────────────────────────────────────────────────────────────────────────────
static int PromptInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int value) && value >= min && value <= max)
            return value;
        Console.WriteLine($"  Please enter an integer between {min} and {max}.");
    }
}
