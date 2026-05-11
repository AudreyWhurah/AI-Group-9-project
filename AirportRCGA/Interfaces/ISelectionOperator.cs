using AirportRCGA.Core;

namespace AirportRCGA.Interfaces;

public interface ISelectionOperator
{
    Individual Select(IReadOnlyList<Individual> population, Random rng);
}
