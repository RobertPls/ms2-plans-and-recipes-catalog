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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Core;

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
                _alimentoFactory.Create("CARNE DE RES", "CARNES", UnidadMedida.Gramo, new InfoNutricional(100, 250, 26, 0, 15)),
                _alimentoFactory.Create("POLLO", "CARNES", UnidadMedida.Gramo, new InfoNutricional(100, 165, 31, 0, 3.6m)),
                _alimentoFactory.Create("PAPA", "TUBERCULOS", UnidadMedida.Gramo, new InfoNutricional(100, 77, 2, 17, 0.1m)),
                _alimentoFactory.Create("ARROZ", "GRANOS", UnidadMedida.Gramo, new InfoNutricional(100, 130, 2.7m, 28, 0.3m)),
                _alimentoFactory.Create("FIDEO", "GRANOS", UnidadMedida.Gramo, new InfoNutricional(100, 131, 5, 25, 1.1m)),
                _alimentoFactory.Create("HUEVO", "PROTEINAS", UnidadMedida.Unidad, new InfoNutricional(60, 93, 7.8m, 0.6m, 6.6m)),
                _alimentoFactory.Create("TOMATE", "VERDURAS", UnidadMedida.Gramo, new InfoNutricional(100, 18, 0.9m, 3.9m, 0.2m)),
                _alimentoFactory.Create("CEBOLLA", "VERDURAS", UnidadMedida.Gramo, new InfoNutricional(100, 40, 1.1m, 9.3m, 0.1m)),
                _alimentoFactory.Create("ZANAHORIA", "VERDURAS", UnidadMedida.Gramo, new InfoNutricional(100, 41, 0.9m, 10, 0.2m)),
                _alimentoFactory.Create("ARVEJA", "LEGUMBRES", UnidadMedida.Gramo, new InfoNutricional(100, 81, 5.4m, 14, 0.4m)),
                _alimentoFactory.Create("HABA", "LEGUMBRES", UnidadMedida.Gramo, new InfoNutricional(100, 88, 8, 14, 0.6m)),
                _alimentoFactory.Create("CHOCLO", "GRANOS", UnidadMedida.Unidad, new InfoNutricional(100, 96, 3.4m, 21, 1.2m)),
                _alimentoFactory.Create("QUESO", "LACTEOS", UnidadMedida.Gramo, new InfoNutricional(100, 350, 25, 1.3m, 28)),
                _alimentoFactory.Create("PAN", "GRANOS", UnidadMedida.Unidad, new InfoNutricional(50, 132, 4.5m, 24.5m, 1.6m)),
                _alimentoFactory.Create("LECHE", "LACTEOS", UnidadMedida.Mililitro, new InfoNutricional(100, 42, 3.4m, 5, 1)),
                _alimentoFactory.Create("PLATANO", "FRUTAS", UnidadMedida.Unidad, new InfoNutricional(100, 89, 1.1m, 23, 0.3m)),
                _alimentoFactory.Create("MANZANA", "FRUTAS", UnidadMedida.Unidad, new InfoNutricional(100, 52, 0.3m, 14, 0.2m)),
                _alimentoFactory.Create("NARANJA", "FRUTAS", UnidadMedida.Unidad, new InfoNutricional(100, 47, 0.9m, 12, 0.1m)),
                _alimentoFactory.Create("LECHUGA", "VERDURAS", UnidadMedida.Gramo, new InfoNutricional(100, 15, 1.4m, 2.9m, 0.2m)),
                _alimentoFactory.Create("QUINUA", "GRANOS", UnidadMedida.Gramo, new InfoNutricional(100, 120, 4.4m, 21, 1.9m)),
                _alimentoFactory.Create("MAIZ MORADO", "GRANOS", UnidadMedida.Gramo, new InfoNutricional(100, 96, 3.4m, 21, 1.2m)),
                _alimentoFactory.Create("AZUCAR", "ENDULZANTES", UnidadMedida.Gramo, new InfoNutricional(100, 387, 0, 100, 0)),
                _alimentoFactory.Create("CANELA", "ESPECIAS", UnidadMedida.Gramo, new InfoNutricional(10, 0, 0, 0, 0)),
                _alimentoFactory.Create("DURAZNO", "FRUTAS", UnidadMedida.Gramo, new InfoNutricional(100, 39, 0.9m, 10, 0.3m)),
                _alimentoFactory.Create("HARINA", "GRANOS", UnidadMedida.Gramo, new InfoNutricional(100, 364, 10, 76, 1)),
                _alimentoFactory.Create("PASAS", "FRUTAS", UnidadMedida.Gramo, new InfoNutricional(100, 299, 3.1m, 79, 0.5m)),
                _alimentoFactory.Create("COCO", "FRUTAS", UnidadMedida.Gramo, new InfoNutricional(100, 354, 3.3m, 15, 33)),
                _alimentoFactory.Create("LOCOTO", "VERDURAS", UnidadMedida.Gramo, new InfoNutricional(100, 40, 1.9m, 9, 0.4m)),
                _alimentoFactory.Create("SALCHICHA", "CARNES", UnidadMedida.Gramo, new InfoNutricional(100, 290, 12, 3, 26)),
                _alimentoFactory.Create("CAMOTE", "TUBERCULOS", UnidadMedida.Gramo, new InfoNutricional(100, 86, 1.6m, 20, 0.1m))
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
            var alimentos = await _context.Alimento.ToListAsync();
            var resId = alimentos.First(a => a.Nombre == "CARNE DE RES").Id;
            var polloId = alimentos.First(a => a.Nombre == "POLLO").Id;
            var papaId = alimentos.First(a => a.Nombre == "PAPA").Id;
            var arrozId = alimentos.First(a => a.Nombre == "ARROZ").Id;
            var fideoId = alimentos.First(a => a.Nombre == "FIDEO").Id;
            var huevoId = alimentos.First(a => a.Nombre == "HUEVO").Id;
            var tomateId = alimentos.First(a => a.Nombre == "TOMATE").Id;
            var cebollaId = alimentos.First(a => a.Nombre == "CEBOLLA").Id;
            var zanahoriaId = alimentos.First(a => a.Nombre == "ZANAHORIA").Id;
            var arvejaId = alimentos.First(a => a.Nombre == "ARVEJA").Id;
            var habaId = alimentos.First(a => a.Nombre == "HABA").Id;
            var chocloId = alimentos.First(a => a.Nombre == "CHOCLO").Id;
            var quesoId = alimentos.First(a => a.Nombre == "QUESO").Id;
            var panId = alimentos.First(a => a.Nombre == "PAN").Id;
            var lecheId = alimentos.First(a => a.Nombre == "LECHE").Id;
            var platanoId = alimentos.First(a => a.Nombre == "PLATANO").Id;
            var manzanaId = alimentos.First(a => a.Nombre == "MANZANA").Id;
            var naranjaId = alimentos.First(a => a.Nombre == "NARANJA").Id;
            var lechugaId = alimentos.First(a => a.Nombre == "LECHUGA").Id;
            var quinuaId = alimentos.First(a => a.Nombre == "QUINUA").Id;
            var maizMoradoId = alimentos.First(a => a.Nombre == "MAIZ MORADO").Id;
            var azucarId = alimentos.First(a => a.Nombre == "AZUCAR").Id;
            var canelaId = alimentos.First(a => a.Nombre == "CANELA").Id;
            var duraznoId = alimentos.First(a => a.Nombre == "DURAZNO").Id;
            var harinaId = alimentos.First(a => a.Nombre == "HARINA").Id;
            var pasasId = alimentos.First(a => a.Nombre == "PASAS").Id;
            var cocoId = alimentos.First(a => a.Nombre == "COCO").Id;
            var locotoId = alimentos.First(a => a.Nombre == "LOCOTO").Id;
            var salchichaId = alimentos.First(a => a.Nombre == "SALCHICHA").Id;
            var camoteId = alimentos.First(a => a.Nombre == "CAMOTE").Id;

            var recetas = new List<Receta>();

            // DESAYUNOS
            var r1 = _recetaFactory.Create("Api con Pastel", "Hervir maiz morado con canela y azucar hasta espesar. Servir caliente acompanado de pastel de queso.");
            r1.AgregarIngrediente(maizMoradoId, new Porcion(100));
            r1.AgregarIngrediente(canelaId, new Porcion(5));
            r1.AgregarIngrediente(azucarId, new Porcion(20));
            recetas.Add(r1);

            var r2 = _recetaFactory.Create("Sandwich de Chola", "Armar sandwich con pan de bola, queso, tomate en rodajas y cebolla. Llevar a la plancha hasta que el queso se derrita.");
            r2.AgregarIngrediente(panId, new Porcion(100));
            r2.AgregarIngrediente(quesoId, new Porcion(80));
            r2.AgregarIngrediente(tomateId, new Porcion(50));
            r2.AgregarIngrediente(cebollaId, new Porcion(30));
            recetas.Add(r2);

            var r3 = _recetaFactory.Create("Mocochinchi", "Hervir durazno deshidratado con canela y azucar. Dejar reposar y servir frio con el durazno rehidratado.");
            r3.AgregarIngrediente(duraznoId, new Porcion(150));
            r3.AgregarIngrediente(canelaId, new Porcion(5));
            r3.AgregarIngrediente(azucarId, new Porcion(20));
            recetas.Add(r3);

            var r4 = _recetaFactory.Create("Cafe con Leche y Pan", "Preparar cafe y mezclar con leche caliente. Servir con pan de bola.");
            r4.AgregarIngrediente(lecheId, new Porcion(200));
            r4.AgregarIngrediente(panId, new Porcion(100));
            recetas.Add(r4);

            var r5 = _recetaFactory.Create("Batido de Platano con Leche", "Licuar platano maduro con leche y azucar. Servir frio.");
            r5.AgregarIngrediente(platanoId, new Porcion(200));
            r5.AgregarIngrediente(lecheId, new Porcion(200));
            r5.AgregarIngrediente(azucarId, new Porcion(15));
            recetas.Add(r5);

            var r6 = _recetaFactory.Create("Bollo de Quinua", "Cocinar quinua y mezclar con queso rallado. Formar bollos y hornear hasta dorar.");
            r6.AgregarIngrediente(quinuaId, new Porcion(100));
            r6.AgregarIngrediente(quesoId, new Porcion(60));
            recetas.Add(r6);

            // ALMUERZOS (principales)
            var r7 = _recetaFactory.Create("Silpancho", "Aplanar la carne de res, empanizar con harina y huevo. Freir y servir sobre una cama de arroz, papa cocida y huevo frito. Acompanar con tomate y cebolla picados.");
            r7.AgregarIngrediente(resId, new Porcion(150));
            r7.AgregarIngrediente(arrozId, new Porcion(100));
            r7.AgregarIngrediente(papaId, new Porcion(100));
            r7.AgregarIngrediente(huevoId, new Porcion(60));
            r7.AgregarIngrediente(harinaId, new Porcion(30));
            r7.AgregarIngrediente(tomateId, new Porcion(50));
            r7.AgregarIngrediente(cebollaId, new Porcion(30));
            recetas.Add(r7);

            var r8 = _recetaFactory.Create("Chairo", "Cocinar carne de res con papa, zanahoria, habas y arvejas. Sazonar con cebolla y locoto. Servir caliente.");
            r8.AgregarIngrediente(resId, new Porcion(100));
            r8.AgregarIngrediente(papaId, new Porcion(100));
            r8.AgregarIngrediente(zanahoriaId, new Porcion(50));
            r8.AgregarIngrediente(habaId, new Porcion(50));
            r8.AgregarIngrediente(arvejaId, new Porcion(50));
            r8.AgregarIngrediente(cebollaId, new Porcion(30));
            r8.AgregarIngrediente(locotoId, new Porcion(10));
            recetas.Add(r8);

            var r9 = _recetaFactory.Create("Pique Macho", "Cortar carne de res y salchichas en trozos. Freir con cebolla, tomate y locoto. Servir sobre papas fritas.");
            r9.AgregarIngrediente(resId, new Porcion(150));
            r9.AgregarIngrediente(salchichaId, new Porcion(100));
            r9.AgregarIngrediente(papaId, new Porcion(150));
            r9.AgregarIngrediente(cebollaId, new Porcion(50));
            r9.AgregarIngrediente(tomateId, new Porcion(50));
            r9.AgregarIngrediente(locotoId, new Porcion(10));
            recetas.Add(r9);

            var r10 = _recetaFactory.Create("Arroz con Pollo", "Sofreir pollo con cebolla y tomate. Agregar arroz y cocinar con caldo hasta que este listo.");
            r10.AgregarIngrediente(polloId, new Porcion(150));
            r10.AgregarIngrediente(arrozId, new Porcion(150));
            r10.AgregarIngrediente(cebollaId, new Porcion(30));
            r10.AgregarIngrediente(tomateId, new Porcion(50));
            recetas.Add(r10);

            var r11 = _recetaFactory.Create("Majadito", "Cocinar arroz con carne de res desmenuzada, cebolla y tomate. Servir con platano frito y huevo.");
            r11.AgregarIngrediente(resId, new Porcion(100));
            r11.AgregarIngrediente(arrozId, new Porcion(150));
            r11.AgregarIngrediente(cebollaId, new Porcion(30));
            r11.AgregarIngrediente(tomateId, new Porcion(50));
            r11.AgregarIngrediente(platanoId, new Porcion(100));
            r11.AgregarIngrediente(huevoId, new Porcion(60));
            recetas.Add(r11);

            var r12 = _recetaFactory.Create("Fideos con Aji de Carne", "Cocinar fideos. Preparar salsa de aji con carne molida, cebolla y tomate. Mezclar y servir.");
            r12.AgregarIngrediente(fideoId, new Porcion(150));
            r12.AgregarIngrediente(resId, new Porcion(100));
            r12.AgregarIngrediente(cebollaId, new Porcion(30));
            r12.AgregarIngrediente(tomateId, new Porcion(50));
            r12.AgregarIngrediente(locotoId, new Porcion(10));
            recetas.Add(r12);

            var r13 = _recetaFactory.Create("Churrasco", "Asar la carne de res a la parrilla. Servir con arroz, papa cocida y ensalada de tomate y cebolla.");
            r13.AgregarIngrediente(resId, new Porcion(200));
            r13.AgregarIngrediente(arrozId, new Porcion(100));
            r13.AgregarIngrediente(papaId, new Porcion(100));
            r13.AgregarIngrediente(tomateId, new Porcion(50));
            r13.AgregarIngrediente(cebollaId, new Porcion(30));
            recetas.Add(r13);

            // POSTRES
            var r14 = _recetaFactory.Create("Arroz con Leche", "Cocinar arroz con leche, azucar y canela hasta que espese. Servir frio o caliente.");
            r14.AgregarIngrediente(arrozId, new Porcion(80));
            r14.AgregarIngrediente(lecheId, new Porcion(300));
            r14.AgregarIngrediente(azucarId, new Porcion(30));
            r14.AgregarIngrediente(canelaId, new Porcion(5));
            recetas.Add(r14);

            var r15 = _recetaFactory.Create("Helado de Canela", "Mezclar leche, azucar y canela. Llevar a la heladora o congelar batiendo cada hora.");
            r15.AgregarIngrediente(lecheId, new Porcion(300));
            r15.AgregarIngrediente(azucarId, new Porcion(40));
            r15.AgregarIngrediente(canelaId, new Porcion(10));
            recetas.Add(r15);

            var r16 = _recetaFactory.Create("Mazamorra", "Cocinar maiz morado con azucar, canela y pasas hasta espesar. Servir con canela espolvoreada.");
            r16.AgregarIngrediente(maizMoradoId, new Porcion(80));
            r16.AgregarIngrediente(azucarId, new Porcion(30));
            r16.AgregarIngrediente(canelaId, new Porcion(5));
            r16.AgregarIngrediente(pasasId, new Porcion(20));
            recetas.Add(r16);

            var r17 = _recetaFactory.Create("Leche Asada", "Mezclar leche, huevo, azucar y canela. Hornear a bano maria hasta que cuaje. Servir frio.");
            r17.AgregarIngrediente(lecheId, new Porcion(300));
            r17.AgregarIngrediente(huevoId, new Porcion(60));
            r17.AgregarIngrediente(azucarId, new Porcion(30));
            r17.AgregarIngrediente(canelaId, new Porcion(5));
            recetas.Add(r17);

            var r18 = _recetaFactory.Create("Bunuelos", "Mezclar harina con huevo, anis y azucar. Freir en aceite caliente hasta dorar. Espolvorear con azucar.");
            r18.AgregarIngrediente(harinaId, new Porcion(150));
            r18.AgregarIngrediente(huevoId, new Porcion(30));
            r18.AgregarIngrediente(azucarId, new Porcion(20));
            recetas.Add(r18);

            var r19 = _recetaFactory.Create("Cocadas", "Mezclar coco rallado con leche condensada y azucar. Formar bolitas y hornear hasta dorar.");
            r19.AgregarIngrediente(cocoId, new Porcion(100));
            r19.AgregarIngrediente(lecheId, new Porcion(100));
            r19.AgregarIngrediente(azucarId, new Porcion(30));
            recetas.Add(r19);

            // CENAS
            var r20 = _recetaFactory.Create("Sopa de Quinua", "Cocinar quinua con zanahoria, arveja, haba y cebolla. Sazonar con sal y servir caliente.");
            r20.AgregarIngrediente(quinuaId, new Porcion(80));
            r20.AgregarIngrediente(zanahoriaId, new Porcion(50));
            r20.AgregarIngrediente(arvejaId, new Porcion(50));
            r20.AgregarIngrediente(habaId, new Porcion(50));
            r20.AgregarIngrediente(cebollaId, new Porcion(30));
            recetas.Add(r20);

            var r21 = _recetaFactory.Create("Tortilla de Papa con Queso", "Rallar papa y mezclar con queso desmenuzado y huevo. Freir en sarten hasta dorar ambos lados.");
            r21.AgregarIngrediente(papaId, new Porcion(150));
            r21.AgregarIngrediente(quesoId, new Porcion(50));
            r21.AgregarIngrediente(huevoId, new Porcion(30));
            recetas.Add(r21);

            var r22 = _recetaFactory.Create("Ensalada Pacena", "Picar tomate, cebolla y locoto. Mezclar con queso desmenuzado y habas cocidas. Alinar con aceite.");
            r22.AgregarIngrediente(tomateId, new Porcion(100));
            r22.AgregarIngrediente(cebollaId, new Porcion(50));
            r22.AgregarIngrediente(locotoId, new Porcion(10));
            r22.AgregarIngrediente(quesoId, new Porcion(50));
            r22.AgregarIngrediente(habaId, new Porcion(50));
            recetas.Add(r22);

            var r23 = _recetaFactory.Create("Caldo de Pollo", "Cocinar pollo con papa, zanahoria y arvejas. Sazonar con cebolla. Servir caliente.");
            r23.AgregarIngrediente(polloId, new Porcion(100));
            r23.AgregarIngrediente(papaId, new Porcion(100));
            r23.AgregarIngrediente(zanahoriaId, new Porcion(50));
            r23.AgregarIngrediente(arvejaId, new Porcion(50));
            r23.AgregarIngrediente(cebollaId, new Porcion(30));
            recetas.Add(r23);

            var r24 = _recetaFactory.Create("Ensalada de Frutas", "Picar platano, manzana y naranja en trozos. Mezclar y servir fresco.");
            r24.AgregarIngrediente(platanoId, new Porcion(100));
            r24.AgregarIngrediente(manzanaId, new Porcion(100));
            r24.AgregarIngrediente(naranjaId, new Porcion(100));
            recetas.Add(r24);

            var r25 = _recetaFactory.Create("Papa Rellena de Queso", "Hacer pure de papa, rellenar con queso, empanizar y freir. Servir con ensalada de lechuga.");
            r25.AgregarIngrediente(papaId, new Porcion(200));
            r25.AgregarIngrediente(quesoId, new Porcion(60));
            r25.AgregarIngrediente(lechugaId, new Porcion(50));
            recetas.Add(r25);

            foreach (var r in recetas)
            {
                await _recetaRepo.CreateAsync(r);
            }

            await _unitOfWork.Commit();
            _logger.LogInformation("Seeded {Count} recetas", recetas.Count);
        }

        private async Task SeedPlanesAlimentarios()
        {
            var recetas = await _context.Receta.Include("_ingredientes").ToListAsync();

            var desayunos = new[]
            {
                recetas.First(r => r.Nombre == "Api con Pastel").Id,
                recetas.First(r => r.Nombre == "Sandwich de Chola").Id,
                recetas.First(r => r.Nombre == "Mocochinchi").Id,
                recetas.First(r => r.Nombre == "Cafe con Leche y Pan").Id,
                recetas.First(r => r.Nombre == "Batido de Platano con Leche").Id,
                recetas.First(r => r.Nombre == "Bollo de Quinua").Id
            };

            var almuerzos = new[]
            {
                recetas.First(r => r.Nombre == "Silpancho").Id,
                recetas.First(r => r.Nombre == "Chairo").Id,
                recetas.First(r => r.Nombre == "Pique Macho").Id,
                recetas.First(r => r.Nombre == "Arroz con Pollo").Id,
                recetas.First(r => r.Nombre == "Majadito").Id,
                recetas.First(r => r.Nombre == "Fideos con Aji de Carne").Id,
                recetas.First(r => r.Nombre == "Churrasco").Id
            };

            var postres = new[]
            {
                recetas.First(r => r.Nombre == "Arroz con Leche").Id,
                recetas.First(r => r.Nombre == "Helado de Canela").Id,
                recetas.First(r => r.Nombre == "Mazamorra").Id,
                recetas.First(r => r.Nombre == "Leche Asada").Id,
                recetas.First(r => r.Nombre == "Bunuelos").Id,
                recetas.First(r => r.Nombre == "Cocadas").Id
            };

            var cenas = new[]
            {
                recetas.First(r => r.Nombre == "Sopa de Quinua").Id,
                recetas.First(r => r.Nombre == "Tortilla de Papa con Queso").Id,
                recetas.First(r => r.Nombre == "Ensalada Pacena").Id,
                recetas.First(r => r.Nombre == "Caldo de Pollo").Id,
                recetas.First(r => r.Nombre == "Ensalada de Frutas").Id,
                recetas.First(r => r.Nombre == "Papa Rellena de Queso").Id
            };

            await CrearPlan("Plan Comidas Bolivianas 15 Dias", TipoDuracion.QUINCENAL, 5, desayunos, almuerzos, postres, cenas);
            await CrearPlan("Plan Comidas Bolivianas 30 Dias", TipoDuracion.MENSUAL, 5, desayunos, almuerzos, postres, cenas);

            await _unitOfWork.Commit();
            _logger.LogInformation("Seeded 2 planes alimentarios (15 and 30 days)");
        }

        private async Task CrearPlan(string nombre, TipoDuracion tipo, int comidasPorDia, RecetaId[] desayunos, RecetaId[] almuerzos, RecetaId[] postres, RecetaId[] cenas)
        {
            var plan = _planFactory.Create(nombre, new DuracionPlan(tipo), comidasPorDia);
            var totalDias = tipo == TipoDuracion.QUINCENAL ? 15 : 30;

            for (int dia = 1; dia <= totalDias; dia++)
            {
                foreach (var tt in Enum.GetValues<TipoTiempoComida>())
                {
                    plan.AgregarTiempoDeComidaADia(dia, tt);
                }

                var day = plan.DiasDelPlan.First(d => d.NumeroDia == dia);

                var desayuno = day.TiemposDeComida.First(t => t.Orden == (int)TipoTiempoComida.Desayuno);
                var mediaManana = day.TiemposDeComida.First(t => t.Orden == (int)TipoTiempoComida.MediaManana);
                var almuerzo = day.TiemposDeComida.First(t => t.Orden == (int)TipoTiempoComida.Almuerzo);
                var merienda = day.TiemposDeComida.First(t => t.Orden == (int)TipoTiempoComida.Merienda);
                var cena = day.TiemposDeComida.First(t => t.Orden == (int)TipoTiempoComida.Cena);

                plan.AsignarRecetaATiempo(dia, desayuno.Id, desayunos[(dia - 1) % desayunos.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, mediaManana.Id, desayunos[(dia - 1) % desayunos.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, almuerzo.Id, almuerzos[(dia - 1) % almuerzos.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, almuerzo.Id, postres[(dia - 1) % postres.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, merienda.Id, cenas[(dia - 1) % cenas.Length], new Racion(1));
                plan.AsignarRecetaATiempo(dia, cena.Id, cenas[(dia - 1) % cenas.Length], new Racion(1));
            }

            await _planRepo.CreateAsync(plan);
        }
    }
}
