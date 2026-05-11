using AirportRCGA.Core;

namespace AirportRCGA.Interfaces;

public interface IMutationOperator
{
    Individual Mutate(Individual individual, Random rng);
}
