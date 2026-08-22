# AGENTS.md

## Comandos

```bash
dotnet build BibliotecaMultimedia.sln          # verificar compilación
dotnet run --project BibliotecaMultimedia.API  # arrancar la API
dotnet ef migrations add <Nombre> -p BibliotecaMultimedia.Infrastructure -s BibliotecaMultimedia.API
```

- Requiere SDK .NET 10 (`net10.0`). No hay proyectos de test, CI ni configuración de lint/format.
- Verificar cambios con `dotnet build`; no existe suite de tests que ejecutar.

## Configuración y secretos

- `BibliotecaMultimedia.API/appsettings.json` y `appsettings.Development.json` están en `.gitignore`: nunca existen en un clone limpio ni deben commitearse.
- Claves requeridas para arrancar: `ConnectionStrings:DefaultConnection` (PostgreSQL), `ConnectionStrings:AzureBlobStorage`, `ConnectionStrings:AzureBlobStorageContainer`, `ConnectionStrings:AzureServiceBus`, `Azure:ServiceBus:TopicName`, `Jwt:Issuer/Audience/Key`. Sin las de Azure, la DI lanza excepción al construir los singletons.
- Al arrancar, `DatabaseSeeder.SeedCatalogoAsync` siembra catálogos (fallos solo se loguean como warning).
- En Development la UI de OpenAPI es Scalar en `/scalar/v1`.

## Arquitectura (clean architecture, 4 proyectos)

Referencias: `Application → Domain`; `Infrastructure → Application + Domain`; `API → todos`. No introducir ciclos ni referencias inversas.

Al agregar una feature vertical completa:

1. DTOs en `Application/DTOs/Peticion/*` y `DTOs/Respuesta/*`.
2. Interfaz en `Application/Interfaces/I*Service.cs`, implementación en `Application/Service/*Service.cs`.
3. Validator FluentValidation en `Application/Validators/*Validator.cs` (se registran automáticamente por escaneo de ensamblado en `AddValidations`).
4. Mapper como clase estática con métodos extensores en `Application/Mappers/*Mapper.cs`.
5. Registrar en DI: `API/Extensions/ApplicationServiceExtensions.cs → AddServices`.
6. Controller delgado en `API/Controllers/V1/` (versionado por URL `api/v1/...` vía Asp.Versioning; también acepta header `X-Api-Version`).

## Convenciones del repo

- Identificadores, comentarios, commits y docs en **español**. DTOs: prefijos `Peticion*` (request), `Respuesta*` (response); filtros de paginación `Filtro*`.
- Los enums se serializan como **strings** (`JsonStringEnumConverter` en Program.cs).
- Paginación: respuesta envuelta en `RespuestaPaginada<T>` con metadata en el header `X-Pagination` (expuesto por CORS para el frontend Angular, repo aparte).
- Persistencia: PostgreSQL/Npgsql; configuraciones Fluent API en `Infrastructure/Configurations` aplicadas automáticamente (`ApplyConfigurationsFromAssembly`); repositorio genérico + UnitOfWork (`_unitOfWork.Repository<T>()` o propiedades tipadas de `IUnitOfWork`).

## Trampas conocidas

- **Soft delete global**: `AuditInterceptor` convierte `Delete` en `UPDATE ... SET DeletedAt` y `AppDbContext.OnModelCreating` aplica query filter `DeletedAt == null` a todo lo que hereda de `BaseEntity`. No agregar filtros por entidad que lo pisen; las consultas nunca ven filas borradas.
- **Dos conceptos "Role"**: roles de Identity (`AppRole`: Admin/User, tablas AspNet*) vs. rol de creador (entidad `Role` de catálogo, DbSet `CreatorRoles`, tabla `roles`). No mezclarlos.
- Autenticación JWT + Identity con claves `Guid`; el `userId` sale de los claims del token — validar ownership en todo recurso por usuario.
- `AUDITORIA.md` es una auditoría histórica (2026-08-20): parte ya está corregida (query filter de soft delete, feature Biblioteca). Útil como lista de bugs potenciales, pero verificar contra el código antes de dar nada por vigente.
