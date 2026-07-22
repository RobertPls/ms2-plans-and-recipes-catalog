<img width="1553" height="754" alt="image" src="https://github.com/user-attachments/assets/99e61c6a-8fba-446b-a38e-9c9b98230f30" />

# ms2-plans-and-recipes-catalog

Microservicio de **Catálogo de Alimentos, Recetas y Planes Alimentarios** para un sistema de nutrición y meal-planning.

## Propósito

Gestionar y exponer un catálogo completo de alimentos con su información nutricional, recetas compuestas por dichos alimentos, y planes alimentarios que organizan recetas en días y tiempos de comida.

## Funcionalidades

### Alimentos
- CRUD de alimentos con información nutricional (calorías, proteínas, carbohidratos, grasas), unidad de medida y categoría.
- Búsqueda de alimentos por categoría.
- Publicación de evento de integración al actualizar datos nutricionales.

### Recetas
- CRUD de recetas con nombre e instrucciones.
- Agregar y remover ingredientes (alimento + porción).
- Cálculo de información nutricional total de una receta basado en sus ingredientes.

### Planes Alimentarios
- CRUD de planes con duración (15 o 30 días) y número de comidas por día.
- Generación automática de los días del plan al crearlo.
- Gestión de tiempos de comida (desayuno, almuerzo, cena, etc.) por día.
- Asignación de recetas con raciones a cada tiempo de comida.
- Obtención de la composición completa del plan (plan > días > tiempos de comida > recetas > ingredientes).
- Publicación de evento de integración al crear un plan.

## Stack Tecnológico

| Componente | Tecnología |
|------------|------------|
| Framework | .NET (ASP.NET Core Web API) |
| Arquitectura | DDD + Clean Architecture |
| Patrón CQRS | MediatR |
| ORM | Entity Framework Core |
| Base de datos | SQLite |
| Documentación API | Swagger / OpenAPI |
| Patrones | Repository, Unit of Work, Factory, Result |

## Estructura del Proyecto

```
ms2-plans-and-recipes-catalog/
├── Domain/          # Entidades, Value Objects, Eventos, Repositorios (interfaces)
├── Application/     # Casos de uso (Commands/Queries), DTOs, Handlers
├── Infrastructure/  # Persistencia (EF Core DbContexts), Repositorios (implementación)
├── WebApi/          # Controladores REST, Middleware, Startup
├── Shared/          # Tipos compartidos (Result, PagedList, etc.)
└── Test/            # Pruebas unitarias e integración
```

## API Endpoints

### Alimentos (`/api/v1/alimentos`)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/alimentos` | Crear alimento |
| GET | `/alimentos/{id}` | Obtener alimento por ID |
| GET | `/alimentos` | Listar alimentos (paginado) |
| GET | `/alimentos/categoria/{categoria}` | Buscar por categoría (paginado) |
| PUT | `/alimentos` | Actualizar alimento |

### Recetas (`/api/v1/recetas`)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/recetas` | Crear receta |
| POST | `/recetas/ingredientes` | Agregar ingredientes |
| DELETE | `/recetas/{recetaId}/ingredientes/{alimentoId}` | Remover ingrediente |
| GET | `/recetas/{id}` | Obtener receta por ID |
| GET | `/recetas` | Listar recetas (paginado) |
| GET | `/recetas/{recetaId}/info-nutricional` | Obtener info nutricional |

### Planes Alimentarios (`/api/v1/planes`)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/planes` | Crear plan |
| POST | `/planes/tiempos-comida` | Agregar tiempo de comida |
| POST | `/planes/asignar-recetas` | Asignar recetas a tiempo de comida |
| DELETE | `/planes/{planId}/tiempos-comida/{tiempoComidaId}/recetas/{recetaId}` | Remover receta de tiempo de comida |
| GET | `/planes/{planId}` | Obtener composición del plan |
| GET | `/planes` | Listar planes |
```
