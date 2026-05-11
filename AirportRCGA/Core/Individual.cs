namespace AirportRCGA.Core;

public sealed class Individual
{
    public double[] Genes { get; }
    public double Fitness { get; set; } = double.MaxValue;

    public Individual(double[] genes)
    {
        Genes = genes;
    }

    public Individual(double[] genes, double fitness)
    {
        Genes = genes;
        Fitness = fitness;
    }

    public Individual Clone()
    {
        var copy = new double[Genes.Length];
        Array.Copy(Genes, copy, Genes.Length);
        return new Individual(copy, Fitness);
    }
}
