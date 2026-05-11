using AirportRCGA.Core;

namespace AirportRCGA.GA;

public sealed class Population
{
    public Individual[] Individuals { get; set; }

    public Population(int size, int numAirports, Random rng)
    {
        int chromosomeLength = 2 * numAirports;
        Individuals = new Individual[size];

        for (int i = 0; i < size; i++)
        {
            var genes = new double[chromosomeLength];

            for (int k = 0; k < numAirports; k++)
            {
                genes[2 * k]     = GaParameters.LatMin + rng.NextDouble() * (GaParameters.LatMax - GaParameters.LatMin);
                genes[2 * k + 1] = GaParameters.LonMin + rng.NextDouble() * (GaParameters.LonMax - GaParameters.LonMin);
            }

            Individuals[i] = new Individual(genes);
        }
    }

    public Individual GetBest()
    {
        Individual best = Individuals[0];
        for (int i = 1; i < Individuals.Length; i++)
        {
            if (Individuals[i].Fitness < best.Fitness)
                best = Individuals[i];
        }
        return best;
    }

    public IReadOnlyList<Individual> AsReadOnly() => Individuals;
}
