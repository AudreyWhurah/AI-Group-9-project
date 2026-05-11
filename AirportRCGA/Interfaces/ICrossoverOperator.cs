using AirportRCGA.Core;

namespace AirportRCGA.Interfaces;

public interface ICrossoverOperator
{
    (Individual Child1, Individual Child2) Cross(Individual parent1, Individual parent2, Random rng);
}
