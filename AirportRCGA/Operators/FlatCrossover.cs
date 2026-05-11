using AirportRCGA.Core;
using AirportRCGA.Interfaces;

namespace AirportRCGA.Operators;

// For each gene j: c_j ~ Uniform(min(p1_j, p2_j), max(p1_j, p2_j))
// Applied twice independently to produce two distinct children.
public sealed class FlatCrossover : ICrossoverOperator
{
    private readonly double _crossoverProbability;

    public FlatCrossover(double crossoverProbability = GaParameters.CrossoverProbability)
    {
        _crossoverProbability = crossoverProbability;
    }

    public (Individual Child1, Individual Child2) Cross(
        Individual parent1, Individual parent2, Random rng)
    {
        if (rng.NextDouble() >= _crossoverProbability)
            return (parent1.Clone(), parent2.Clone());

        int length = parent1.Genes.Length;
        var genes1 = new double[length];
        var genes2 = new double[length];

        for (int j = 0; j < length; j++)
        {
            double lo = Math.Min(parent1.Genes[j], parent2.Genes[j]);
            double hi = Math.Max(parent1.Genes[j], parent2.Genes[j]);

            if (hi - lo < 1e-15)
            {
                // Parents share the same value — degenerate interval
                genes1[j] = lo;
                genes2[j] = lo;
            }
            else
            {
                genes1[j] = lo + rng.NextDouble() * (hi - lo);
                genes2[j] = lo + rng.NextDouble() * (hi - lo);
            }
        }

        return (new Individual(genes1), new Individual(genes2));
    }
}
