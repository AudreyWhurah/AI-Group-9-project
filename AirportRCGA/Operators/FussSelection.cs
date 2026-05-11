using AirportRCGA.Core;
using AirportRCGA.Interfaces;

namespace AirportRCGA.Operators;

// Fitness Uniform Selection Scheme (FUSS) — minimisation variant.
// Samples u ~ Uniform(f_min, f_max), returns argmin_i |fitness_i - u|.
// Equal probability per unit of the fitness axis preserves diversity.
public sealed class FussSelection : ISelectionOperator
{
    public Individual Select(IReadOnlyList<Individual> population, Random rng)
    {
        double fitnessMin = double.MaxValue;
        double fitnessMax = double.MinValue;

        foreach (Individual individual in population)
        {
            if (individual.Fitness < fitnessMin) fitnessMin = individual.Fitness;
            if (individual.Fitness > fitnessMax) fitnessMax = individual.Fitness;
        }

        if (fitnessMax - fitnessMin < GaParameters.FussEqualityThreshold)
            return population[rng.Next(population.Count)];

        double target = fitnessMin + rng.NextDouble() * (fitnessMax - fitnessMin);

        Individual best = population[0];
        double bestDelta = Math.Abs(population[0].Fitness - target);

        for (int i = 1; i < population.Count; i++)
        {
            double delta = Math.Abs(population[i].Fitness - target);
            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = population[i];
            }
        }

        return best;
    }
}
