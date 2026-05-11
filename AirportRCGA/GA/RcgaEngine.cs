using AirportRCGA.Core;
using AirportRCGA.Interfaces;

namespace AirportRCGA.GA;

public sealed class RcgaEngine
{
    private readonly ICrossoverOperator _crossover;
    private readonly IMutationOperator _mutation;
    private readonly ISelectionOperator _selection;
    private readonly ObjectiveFunction _objectiveFunction;
    private readonly Random _rng;

    public RcgaEngine(
        ICrossoverOperator crossover,
        IMutationOperator mutation,
        ISelectionOperator selection,
        ObjectiveFunction objectiveFunction,
        Random rng)
    {
        _crossover = crossover;
        _mutation = mutation;
        _selection = selection;
        _objectiveFunction = objectiveFunction;
        _rng = rng;
    }

    public (Individual Best, List<(int Generation, double BestFitness)> ConvergenceLog)
        Run(int numAirports, Action<int, double>? generationCallback = null)
    {
        var population = new Population(GaParameters.PopulationSize, numAirports, _rng);

        foreach (Individual individual in population.Individuals)
            individual.Fitness = _objectiveFunction.Evaluate(individual.Genes, numAirports);

        var convergenceLog = new List<(int, double)>(GaParameters.MaxGenerations);

        for (int generation = 0; generation < GaParameters.MaxGenerations; generation++)
        {
            Individual elite = population.GetBest();

            convergenceLog.Add((generation, elite.Fitness));
            generationCallback?.Invoke(generation, elite.Fitness);

            var newIndividuals = new Individual[GaParameters.PopulationSize];
            newIndividuals[0] = elite.Clone(); // elitism — best survives unchanged

            int filled = 1;
            IReadOnlyList<Individual> readOnlyPop = population.AsReadOnly();

            while (filled < GaParameters.PopulationSize)
            {
                Individual parent1 = _selection.Select(readOnlyPop, _rng);
                Individual parent2 = _selection.Select(readOnlyPop, _rng);

                var (child1, child2) = _crossover.Cross(parent1, parent2, _rng);

                child1 = _mutation.Mutate(child1, _rng);
                child2 = _mutation.Mutate(child2, _rng);

                child1.Fitness = _objectiveFunction.Evaluate(child1.Genes, numAirports);
                newIndividuals[filled++] = child1;

                if (filled < GaParameters.PopulationSize)
                {
                    child2.Fitness = _objectiveFunction.Evaluate(child2.Genes, numAirports);
                    newIndividuals[filled++] = child2;
                }
            }

            population.Individuals = newIndividuals;
        }

        Individual finalBest = population.GetBest();
        return (finalBest, convergenceLog);
    }
}
