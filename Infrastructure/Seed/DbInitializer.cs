using Catalog.Domain;
using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Factory.Receta;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.EntityFramework.Context;
using Catalog.Shared.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Seed
{
    public class DbInitializer : IDbInitializer
    {
        private readonly WriteDbContext _context;
        private readonly IAlimentoFactory _alimentoFactory;
        private readonly IRecetaFactory _recetaFactory;
        private readonly IPlanAlimentarioFactory _planFactory;
        private readonly IAlimentoRepository _alimentoRepo;
        private readonly IRecetaRepository _recetaRepo;
        private readonly IPlanAlimentarioRepository _planRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            WriteDbContext context,
            IAlimentoFactory alimentoFactory,
            IRecetaFactory recetaFactory,
            IPlanAlimentarioFactory planFactory,
            IAlimentoRepository alimentoRepo,
            IRecetaRepository recetaRepo,
            IPlanAlimentarioRepository planRepo,
            IUnitOfWork unitOfWork,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _alimentoFactory = alimentoFactory;
            _recetaFactory = recetaFactory;
            _planFactory = planFactory;
            _alimentoRepo = alimentoRepo;
            _recetaRepo = recetaRepo;
            _planRepo = planRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Initialize()
        {
            if (await _context.Alimento.AnyAsync())
            {
                _logger.LogInformation("Database already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding database...");

            await SeedAlimentos();
            await SeedRecetas();
            await SeedPlanesAlimentarios();

            _logger.LogInformation("Database seeding completed.");
        }

        private async Task SeedAlimentos()
        {
            var alimentos = new[]
            {
                _alimentoFactory.Create("Pollo", "Carnes", new InfoNutricional(100, 165, 31, 0, 3.6m)),
                _alimentoFactory.Create("Arroz", "Granos", new InfoNutricional(100, 130, 2.7m, 28, 0.3m)),
                _alimentoFactory.Create("Frijoles", "Legumbres", new InfoNutricional(100, 152, 9, 27, 0.8m)),
                _alimentoFactory.Create("Lechuga", "Verduras", new InfoNutricional(100, 15, 1.4m, 2.9m, 0.2m)),
                _alimentoFactory.Create("Platano", "Frutas", new InfoNutricional(100, 89, 1.1m, 23, 0.3m)),
                _alimentoFactory.Create("Huevo", "Proteinas", new InfoNutricional(100, 155, 13, 1.1m, 11)),
                _alimentoFactory.Create("Aguacate", "Verduras", new InfoNutricional(100, 160, 2, 8.5m, 15)),
                _alimentoFactory.Create("Pasta", "Granos", new InfoNutricional(100, 131, 5, 25, 1.1m))
            };

            foreach (var alimento in alimentos)
            {
                await _alimentoRepo.CreateAsync(alimento);
            }

            await _unitOfWork.Commit();
            _logger.LogInformation("Seeded {Count} alimentos", alimentos.Length);
        }

        private async Task SeedRecetas()
        {
            var alimentos = await _alimentoRepo.FindAllAsync();
            var polloId = alimentos.First(a => a.Nombre == "Pollo").Id;
            var arrozId = alimentos.First(a => a.Nombre == "Arroz").Id;
            var frijolesId = alimentos.First(a => a.Nombre == "Frijoles").Id;
            var lechugaId = alimentos.First(a => a.Nombre == "Lechuga").Id;
            var platanoId = alimentos.First(a => a.Nombre == "Platano").Id;
            var huevoId = alimentos.First(a => a.Nombre == "Huevo").Id;
            var aguacateId = alimentos.First(a => a.Nombre == "Aguacate").Id;
            var pastaId = alimentos.First(a => a.Nombre == "Pasta").Id;

            var recetas = new List<Receta>();

            var r1 = _recetaFactory.Create("Pechuga a la Plancha", "Sazonar la pechuga con sal y pimienta. Cocinar en sarten caliente 6 min por lado. Servir con arroz y aguacate.");
            r1.AgregarIngrediente(polloId, new Porcion(200, "g"));
            r1.AgregarIngrediente(arrozId, new Porcion(150, "g"));
            r1.AgregarIngrediente(aguacateId, new Porcion(50, "g"));
            recetas.Add(r1);

            var r2 = _recetaFactory.Create("Ensalada de Frijoles", "Mezclar frijoles cocidos con lechuga picada y huevo duro. Alinar con aceite de oliva al gusto.");
            r2.AgregarIngrediente(frijolesId, new Porcion(150, "g"));
            r2.AgregarIngrediente(lechugaId, new Porcion(100, "g"));
            r2.AgregarIngrediente(huevoId, new Porcion(60, "g"));
            recetas.Add(r2);

            var r3 = _recetaFactory.Create("Pasta con Pollo", "Cocinar la pasta. Saltear el pollo en cubos. Mezclar con la pasta y agregar aguacate en trozos.");
            r3.AgregarIngrediente(pastaId, new Porcion(200, "g"));
            r3.AgregarIngrediente(polloId, new Porcion(150, "g"));
            r3.AgregarIngrediente(aguacateId, new Porcion(80, "g"));
            recetas.Add(r3);

            var r4 = _recetaFactory.Create("Arroz con Pollo", "Cocinar arroz y mezclar con pollo desmenuzado. Agregar condimentos al gusto.");
            r4.AgregarIngrediente(polloId, new Porcion(150, "g"));
            r4.AgregarIngrediente(arrozId, new Porcion(150, "g"));
            recetas.Add(r4);

            var r5 = _recetaFactory.Create("Ensalada de Lechuga y Huevo", "Picar lechuga y mezclar con huevo duro picado. Alinar con limon.");
            r5.AgregarIngrediente(lechugaId, new Porcion(100, "g"));
            r5.AgregarIngrediente(huevoId, new Porcion(60, "g"));
            recetas.Add(r5);

            var r6 = _recetaFactory.Create("Pasta con Frijoles", "Cocinar pasta y mezclar con frijoles cocidos. Agregar aguacate en trozos.");
            r6.AgregarIngrediente(pastaId, new Porcion(200, "g"));
            r6.AgregarIngrediente(frijolesId, new Porcion(100, "g"));
            r6.AgregarIngrediente(aguacateId, new Porcion(50, "g"));
            recetas.Add(r6);

            var r7 = _recetaFactory.Create("Batido de Platano", "Licuar platano con huevo. Servir frio.");
            r7.AgregarIngrediente(platanoId, new Porcion(200, "g"));
            r7.AgregarIngrediente(huevoId, new Porcion(30, "g"));
            recetas.Add(r7);

            var r8 = _recetaFactory.Create("Aguacate Relleno de Pollo", "Mezclar pollo desmenuzado con lechuga y rellenar el aguacate.");
            r8.AgregarIngrediente(polloId, new Porcion(100, "g"));
            r8.AgregarIngrediente(aguacateId, new Porcion(150, "g"));
            r8.AgregarIngrediente(lechugaId, new Porcion(50, "g"));
            recetas.Add(r8);

            var r9 = _recetaFactory.Create("Arroz con Frijoles", "Cocinar arroz y frijoles juntos con verduras.");
            r9.AgregarIngrediente(arrozId, new Porcion(150, "g"));
            r9.AgregarIngrediente(frijolesId, new Porcion(100, "g"));
            r9.AgregarIngrediente(lechugaId, new Porcion(50, "g"));
            recetas.Add(r9);

            var r10 = _recetaFactory.Create("Tortilla de Huevo con Verduras", "Batir huevo y mezclar con lechuga picada. Cocinar en sarten.");
            r10.AgregarIngrediente(huevoId, new Porcion(120, "g"));
            r10.AgregarIngrediente(lechugaId, new Porcion(50, "g"));
            recetas.Add(r10);

            var r11 = _recetaFactory.Create("Ensalada de Platano y Aguacate", "Cortar platano y aguacate en trozos. Mezclar con limon.");
            r11.AgregarIngrediente(platanoId, new Porcion(150, "g"));
            r11.AgregarIngrediente(aguacateId, new Porcion(100, "g"));
            recetas.Add(r11);

            foreach (var r in recetas)
            {
                await _recetaRepo.CreateAsync(r);
            }

            await _unitOfWork.Commit();
            _logger.LogInformation("Seeded {Count} recetas", recetas.Count);
        }

        private async Task SeedPlanesAlimentarios()
        {
            var recetas = await _recetaRepo.FindAllAsync();

            var desayunos = new[]
            {
                recetas.First(r => r.Nombre == "Batido de Platano").Id,
                recetas.First(r => r.Nombre == "Tortilla de Huevo con Verduras").Id,
                recetas.First(r => r.Nombre == "Ensalada de Platano y Aguacate").Id
            };

            var almuerzos = new[]
            {
                recetas.First(r => r.Nombre == "Pechuga a la Plancha").Id,
                recetas.First(r => r.Nombre == "Pasta con Pollo").Id,
                recetas.First(r => r.Nombre == "Arroz con Pollo").Id,
                recetas.First(r => r.Nombre == "Pasta con Frijoles").Id,
                recetas.First(r => r.Nombre == "Arroz con Frijoles").Id
            };

            var cenas = new[]
            {
                recetas.First(r => r.Nombre == "Ensalada de Frijoles").Id,
                recetas.First(r => r.Nombre == "Ensalada de Lechuga y Huevo").Id,
                recetas.First(r => r.Nombre == "Aguacate Relleno de Pollo").Id
            };

            await CrearPlan("Plan Saludable 15 Dias", TipoDuracion.QUINCENAL, desayunos, almuerzos, cenas);
            await CrearPlan("Plan Completo 30 Dias", TipoDuracion.MENSUAL, desayunos, almuerzos, cenas);

            await _unitOfWork.Commit();
            _logger.LogInformation("Seeded 2 planes alimentarios (15 and 30 days)");
        }

        private async Task CrearPlan(string nombre, TipoDuracion tipo, RecetaId[] desayunos, RecetaId[] almuerzos, RecetaId[] cenas)
        {
            var plan = _planFactory.Create(nombre, new DuracionPlan(tipo), DateTime.UtcNow.AddDays(1));
            var totalDias = tipo == TipoDuracion.QUINCENAL ? 15 : 30;

            for (int dia = 1; dia <= totalDias; dia++)
            {
                plan.AgregarTiempoDeComidaADia(dia, "Desayuno", 1);
                plan.AgregarTiempoDeComidaADia(dia, "Almuerzo", 2);
                plan.AgregarTiempoDeComidaADia(dia, "Cena", 3);

                var day = plan.DiasDelPlan.First(d => d.NumeroDia == dia);

                var desayuno = day.TiemposDeComida.First(t => t.Orden == 1);
                var almuerzo = day.TiemposDeComida.First(t => t.Orden == 2);
                var cena = day.TiemposDeComida.First(t => t.Orden == 3);

                plan.AsignarRecetaATiempo(dia, desayuno.Id, desayunos[(dia - 1) % desayunos.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, almuerzo.Id, almuerzos[(dia - 1) % almuerzos.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, cena.Id, cenas[(dia - 1) % cenas.Length], new Racion(1));
            }

            await _planRepo.CreateAsync(plan);
        }
    }
}
