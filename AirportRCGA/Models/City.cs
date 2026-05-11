using System.Collections.Immutable;

namespace AirportRCGA.Models;

public record City(string Name, string State, long Population, double LatRad, double LonRad);

public static class CityData
{
    public static readonly ImmutableArray<City> Cities = ImmutableArray.Create(
        new City("Lagos",       "Lagos",  14_800_000, 0.112661, 0.059064),
        new City("Ibadan",      "Oyo",     3_649_000, 0.128763, 0.068171),
        new City("Benin City",  "Edo",     1_782_000, 0.110567, 0.098210),
        new City("Ilorin",      "Kwara",     847_000, 0.148353, 0.079412),
        new City("Warri",       "Delta",     720_000, 0.096285, 0.100356),
        new City("Abeokuta",    "Ogun",      735_000, 0.124891, 0.058383),
        new City("Ogbomosho",   "Oyo",       897_000, 0.141953, 0.074176),
        new City("Akure",       "Ondo",      730_000, 0.126582, 0.090712),
        new City("Osogbo",      "Osun",      760_000, 0.135554, 0.079704),
        new City("Ado-Ekiti",   "Ekiti",     800_000, 0.133013, 0.091131),
        new City("Asaba",       "Delta",     320_000, 0.107919, 0.117810),
        new City("Lokoja",      "Kogi",      300_000, 0.136136, 0.117518),
        new City("Ile-Ife",     "Osun",      501_000, 0.130318, 0.079704),
        new City("Sagamu",      "Ogun",      350_000, 0.119325, 0.063664),
        new City("Ikorodu",     "Lagos",     535_000, 0.115530, 0.061266),
        new City("Offa",        "Kwara",     290_000, 0.142244, 0.082322),
        new City("Okene",       "Kogi",      480_000, 0.131772, 0.108792),
        new City("Ondo City",   "Ondo",      441_000, 0.123627, 0.084357),
        new City("Sapele",      "Delta",     300_000, 0.102974, 0.099192),
        new City("Oyo",         "Oyo",       736_000, 0.137008, 0.068649)
    );
}
