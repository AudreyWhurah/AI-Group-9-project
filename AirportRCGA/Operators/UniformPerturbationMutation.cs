using AirportRCGA.Core;
using AirportRCGA.Interfaces;

namespace AirportRCGA.Operators;

// For each gene j with probability P_m = 1/(2N):
//   g'_j = lo + u * (hi - lo),  u ~ Uniform(0,1)
// Even-indexed genes are latitudes; odd-indexed are longitudes.
public sealed class UniformPerturbationMutation : IMutationOperator
{
    private readonly double _mutationRate;

    public UniformPerturbationMutation(int numAirports)
    {
        _mutationRate = GaParameters.MutationRate(numAirports);
    }

    public Individual Mutate(Individual individual, Random rng)
    {
        // Clone so the parent individual's gene array is never modified
        var newGenes = new double[individual.Genes.Length];
        Array.Copy(individual.Genes, newGenes, individual.Genes.Length);

        for (int j = 0; j < newGenes.Length; j++)
        {
            if (rng.NextDouble() < _mutationRate)
            {
                double lo = (j % 2 == 0) ? GaParameters.LatMin : GaParameters.LonMin;
                double hi = (j % 2 == 0) ? GaParameters.LatMax : GaParameters.LonMax;
                newGenes[j] = lo + rng.NextDouble() * (hi - lo);
            }
        }

        return new Individual(newGenes);
    }
}
