using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            // Seed Materials if empty
            if (!context.Materiaux.Any())
            {
                context.Materiaux.AddRange(
                    new Materiau { Nom = "Béton", FacteurEmission = 0.12m },
                    new Materiau { Id = 2, Nom = "Acier", FacteurEmission = 1.85m },
                    new Materiau { Id = 3, Nom = "Bois", FacteurEmission = 0.3m },
                    new Materiau { Id = 4, Nom = "Vitrage", FacteurEmission = 0.8m },
                    new Materiau { Id = 5, Nom = "Isolant", FacteurEmission = 1.2m }
                );
                context.SaveChanges();
            }

            // Seed Energy Factors if empty
            if (!context.FacteursEnergie.Any())
            {
                context.FacteursEnergie.AddRange(
                    new FacteurEnergie { TypeEnergie = "Électricité", FacteurEmission = 0.057m, Unite = "kWh" },
                    new FacteurEnergie { TypeEnergie = "Gaz naturel", FacteurEmission = 0.227m, Unite = "kWh" },
                    new FacteurEnergie { TypeEnergie = "Fioul", FacteurEmission = 0.324m, Unite = "kWh" },
                    new FacteurEnergie { TypeEnergie = "Géothermie", FacteurEmission = 0.015m, Unite = "kWh" }
                );
                context.SaveChanges();
            }
        }
    }
}
