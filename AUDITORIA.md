# Auditoría Integral — BibliotecaMultimediaBackend

Fecha: 2026-08-20
Alcance: BibliotecaMultimediaBackend (.NET 10, ASP.NET Core, EF Core 10, PostgreSQL, Clean Architecture N-Layers)
Método: análisis estático de código + revisión de la migración 20260607060426 y del contrato consumido por `BibliotecaMultimediaFrontend` (Angular). Sin ejecución contra BD.

---

## A. Resumen ejecutivo

El backend está **bien estructurado en 4 capas** (API / Application / Domain / Infrastructure), con inyección de dependencias, validación FluentValidation, manejo global de excepciones con ProblemDetails, versionado de API, JWT + Identity y una migración coherente con las entidades. El patrón GenericRepository + UnitOfWork es consistente y se usa de forma homogénea en los catálogos.

Sin embargo, el sistema **no implementa la funcionalidad central del dominio**: la **biblioteca del usuario**. La entidad `UserItem` existe en la BD, pero **no hay controller, service, DTOs, mappers ni validators** para que un usuario agregue contenido a su biblioteca, lo puntúe, lo marque como favorito o lo organice. Esto es la omisión más grave y coincide con el requisito funcional del prompt.

Además se detectaron **bugs reales** que hoy producen operaciones "no-op" o fallos de integridad:

- `ItemImageService.ActualizarImagenAsync` actualiza una entidad **desconectada** (AsNoTracking) → el UPDATE no se persiste.
- `ItemImageService.EliminarImagenAsync` **nunca llama a `Eliminar`** → borrar imagen no hace nada.
- `PlataformaService.EliminarPlataforma` **no llama a `SaveChangesAsync`** → la eliminación no se persiste.
- `ItemImagesController.SubirImagenChunk` no tiene `[HttpPost]`/ruta ni `[Authorize]` → endpoint roto y subida pública a Blob Storage.
- `ItemMapper.MapToDto` de imágenes mapea `Id = image.ItemId` → el Id del DTO siempre es el del item.
- `ItemService.AgregarItem` inserta `ItemCreator` con `RoleId = Guid.Empty` → violación de FK si no existe ese rol (catálogo `roles` sin seed).
- El soft-delete del `AuditInterceptor` marca `DeletedAt`, pero **no hay query filter global** que excluya registros borrados → "borrar" no oculta nada.
- `ItemValidator` valida `Rating` (0-5) pero `Item` **no tiene** la propiedad `Rating` ni `IsFavorite`; esos campos del DTO son muertos y confunden el contrato.
- `ActualizarItem` no actualiza `GenreIds`/`CreatorIds`.
- Paginación sin validación de mínimos y `OrdenarPor`/`OrdenDescendente` declarados pero **nunca usados**.

**Prioridad de corrección:** 1) implementar biblioteca y su ownership, 2) bugs de persistencia/imagenes/seguridad, 3) integridad de datos (soft-delete, FKs, índices), 4) arquitectura/perf.

**Qué está bien:** separación de capas, manejo de errores centralizado, JWT con claims de rol y `NameIdentifier`, unique indexes en `user_items`, `item_genres`, `item_creators`, índice parcial de imagen primaria, enums persistidos como string con `ConsumptionStatus`, catálogos como tablas, uso correcto de `CreatedAtAction`, uso de `AsNoTracking` en consultas de lectura y proyección a DTO en memoria.

---

## B. Problemas críticos

| Severidad | Ubicación | Problema | Impacto | Solución |
|---|---|---|---|---|
| 🔴 CRÍTICO | Backend completo | **La biblioteca del usuario no existe** (`UserItem` sin controller/service/DTOs). | El requisito central del producto (biblioteca, favoritos, rating, categorías personales) no está disponible. | Implementar feature completa de Biblioteca con ownership desde el JWT (ver sección E y fixes F1). |
| 🔴 CRÍTICO | `ItemImageService.ActualizarImagenAsync` (L86-88) | Actualiza entidad obtenida con `disableTracking: true`; el `SaveChanges` no tiene nada que guardar. | Actualizar imagen no persiste cambios (bug no-op). | Traer con tracking o llamar a `Actualizar()`. (F2) |
| 🔴 CRÍTICO | `ItemImageService.EliminarImagenAsync` (L94-99) | Nunca llama `Eliminar`; solo `SaveChanges`. | Borrar imagen no elimina nada. | Llamar `Eliminar(imagen)` antes de guardar. (F2) |
| 🔴 CRÍTICO | `PlataformaService.EliminarPlataforma` (L86-91) | Falta `SaveChangesAsync`. | Eliminar plataforma no se persiste. | Agregar `SaveChangesAsync`. (F3) |
| 🔴 CRÍTICO | `ItemImagesController.SubirImagenChunk` (L21) | Sin `[HttpPost]`, sin ruta `{itemId}` y sin `[Authorize]`. | Endpoint no ruteable como se espera y subida anónima de archivos a Blob Storage (contenedor público). | Agregar verbo, ruta y autorización Admin. (F4) |
| 🔴 CRÍTICO | `AuditInterceptor` + `GenericRepository` | Soft-delete marca `DeletedAt` pero **no hay query filter** que excluya esos registros. | "Eliminar" items/catálogos no los oculta de las consultas. | Añadir query filter global `DeletedAt == null` en `OnModelCreating`. (F9) |
| 🟠 ALTO | `ItemService.AgregarItem` (L86) | `ItemCreator.RoleId = Guid.Empty` + catálogo `roles` sin seed. | Si no existe un rol con ese GUID (caso normal), falla la FK → crear item con creadores da error. | Validar existencia de FKs y usar un rol real del catálogo; seed de `roles`/`media_types`. (F6) |
| 🟠 ALTO | `ImagenMapper.MapToDto` (L23) | `Id = image.ItemId` en lugar de `image.Id`. | El Id de la imagen en el DTO es incorrecto. | Corregir mapeo. (F5) |
| 🟠 ALTO | `ItemService.AgregarItem` / `ActualizarItem` | No se valida que `MediaTypeId`, `FormatId`, `PlatformId`, `GenreIds`, `CreatorIds` existan. | FKs inexistentes → excepción 500 en vez de 400/404. | Validar existencia de referencias. (F6/F7) |

---

## C. Arquitectura

**Arquitectura actual:** N-Layers limpio, 4 proyectos:

```
BibliotecaMultimedia.API ──► Application ──► Domain
        └──────────────────────► Infrastructure ──► Application
                                                  └─► Domain
```

- **Domain:** modelos, enums, interfaces `IGenericRepository<T>`, `IUnitOfWork`, excepciones, constantes. **Problema:** depende de `Microsoft.AspNetCore.Identity`/`Identity.EntityFrameworkCore` (para `User : IdentityUser<Guid>`) — acoplamiento de infraestructura en el núcleo del dominio. Aceptable pragmáticamente en apps pequeñas con Identity, pero debería documentarse; el `User` de dominio no se usa en reglas de negocio puras.
- **Application:** servicios, DTOs, mappers estáticos, validators FluentValidation, excepciones de aplicación. Bien.
- **Infrastructure:** `AppDbContext`, configuraciones Fluent API, repositorio genérico, UnitOfWork, interceptores (auditoría y sesión de usuario), servicios Azure (BlobStorage, ServiceBus). Bien.
- **API:** controllers v1, extensions de DI, `GlobalExceptionHandler`.

**Dependencias / acoplamiento:**
- Referencias correctas (Application→Domain, Infrastructure→Application+Domain, API→todos). Sin ciclos.
- `Application/Service` conoce `IUnitOfWork`/repositorios del Domain (ok, es la capa que orquesta).
- `Application/Exceptions/NotFoundException` **duplica** `Domain/Exceptions/NotFoundException`. Confuso: los servicios importan indistintamente una u otra. Unificar.
- `AppDbContext.Roles` (custom `Role`) **oculta** la propiedad heredada `IdentityDbContext.Roles` (warning CS0114). Hay **dos conceptos de rol**: `AspNetRoles` (Identity: Admin/User) y la tabla `roles` (Rol de creador: Director/Autor/Compositor/Desarrollador) con el mismo nombre `Role`. Colisión de nomenclatura → riesgo de confusión. Renombrar el DbSet a `CreatorRoles` (sin impacto en tabla vía `ToTable`).

**Responsabilidades:** los controllers son delgados (delegan al servicio) — correcto. No hay lógica de negocio en controllers. La lógica de negocio vive en los servicios (bien). Los repositorios son genéricos y no contienen lógica (bien).

**Arquitectura recomendada:** conservar N-Layers actual (es proporcional al problema). No introducir CQRS/MediatR/Eventos de dominio — sería sobreingeniería. Extraer el `userId` del contexto JWT mediante un helper (`ClaimsPrincipal.GetUserId()`) para centralizar ownership. Considerar mover la excepción `NotFound`/`BusinessRule` a Domain y usarla en todas las capas.

---

## D. Modelo de dominio

**Entidades actuales:**

```
BaseEntity (Id, CreatedAt, UpdatedAt, DeletedAt, IsDeleted)
├── Item (MediaTypeId, FormatId, PlatformId?, Title, ReleaseDate, Metadata jsonb)
│     ├── ItemGenre (ItemId, GenreId)  [unique (ItemId,GenreId)]
│     ├── ItemCreator (ItemId, CreatorId, RoleId) [unique (ItemId,CreatorId,RoleId)]
│     └── ItemImage (ItemId, ImageUrl, IsPrimary) [único parcial IsPrimary=true]
├── MediaType / Format / Platform / Genre / Creator / Role  → catálogos (tablas)
└── UserItem (UserId, ItemId, Status, Progress, IsFavorite, PersonalRating, Review,
              DateAdded, StartedAt, FinishedAt, IsPrivate)  [unique (UserId,ItemId)]
User : IdentityUser<Guid> (UserItems)
```

**Aciertos:** `UserItem` como tabla intermedia con metadatos por usuario; unique index `(UserId, ItemId)` evita duplicados; catálogos como tablas (extensibles); `ConsumptionStatus` como enum persistido en string (valores cerrados y estables).

**Problemas del modelo:**
1. `Item.Rating` y `Item.IsFavorite` **no existen** en la entidad, pero están en `PeticionCrearItemDto`, `PeticionActualizarItemDto` y `RespuestaItemDto` → campos fantasma que confunden. Esos atributos pertenecen a `UserItem`. Eliminarlos de los DTOs de Item.
2. `Item.Metadata` como `JsonDocument` es problemático (no desechable en entidad; mejor `JsonElement` o un value object). Funciona con Npgsql jsonb, pero se recomienda `JsonElement`.
3. `UserItem.DateAdded` duplica a `BaseEntity.CreatedAt` conceptualmente. Se puede conservar `DateAdded` como semántica de negocio o eliminarlo y usar `CreatedAt`.
4. `Role` (rol de creador) choca de nombre con los roles de Identity. Renombrar a `CreatorRole` para claridad (requiere migración de nombres; opcional).
5. `ItemGenre`/`ItemCreator` usan PK `Id` surrogate (Guid) además del índice único compuesto. Correcto; el índice único es la verdadera identidad lógica.

**Modelo recomendado (biblioteca):**

```
User (IdentityUser<Guid>)
  └── UserItem (UserId, ItemId, Status, Progress, IsFavorite, PersonalRating,
                Review, DateAdded, StartedAt, FinishedAt, IsPrivate)
        └── Item (catálogo global, gestionado por Admin)
```

Con constraints: unique `(UserId, ItemId)`; FK `UserId`→User Cascade; FK `ItemId`→Item **Restrict** (o soft-delete que no rompa la biblioteca); `PersonalRating` en [1,5]; `ConsumptionStatus` cerrado. Ver sección E.

---

## E. Biblioteca del usuario (diseño recomendado + fix implementado)

**Modelo correcto:** una entidad intermedia explícita `UserItem` (ya existe) que representa "el artículo dentro de la biblioteca del usuario". No basta una relación simple User↔Item porque se necesitan metadatos por usuario (estado, progreso, favorito, rating, review, fechas, privacidad).

**Restricciones:**
- Un usuario no puede agregar el mismo artículo dos veces → **unique index `(UserId, ItemId)`** (ya existe en BD).
- El rating pertenece a `(usuario, artículo)` → vive en `UserItem.PersonalRating`.
- Favoritos por usuario → `UserItem.IsFavorite`.
- Ownership: **todas las operaciones de biblioteca usan `UserId` del JWT, nunca del body/query**. El frontend no puede elegir de quién es la biblioteca → evita IDOR/BOLA.
- Eliminar un artículo: con soft-delete global, `Item` conserva su fila pero debe excluirse de consultas (F9). Las `UserItem` que lo referencian se mantienen (no cascada destructiva). Opción alternativa documentada: hard-delete con cascade de dependencias + archivar.

**Operaciones implementadas (fix F1):** `POST /api/v1/Biblioteca`, `GET /api/v1/Biblioteca/paginado`, `GET /api/v1/Biblioteca/{id}`, `PUT /api/v1/Biblioteca/{id}`, `DELETE /api/v1/Biblioteca/{id}`, `PUT /api/v1/Biblioteca/{id}/favorito`, `PUT /api/v1/Biblioteca/{id}/rating`.

---

## F. Enums vs catálogos

| Concepto | Actual | Recomendación | Motivo |
|---|---|---|---|
| `ConsumptionStatus` (Pendiente/EnProgreso/Completado/Abandonado) | Enum (string en BD) | **Enum** | Valores cerrados, estables, sin metadata, no los administra la app. |
| `AppRole` (Admin/User) | Enum | **Enum** | Roles de seguridad, cerrados, no administrables. |
| `MediaType` (Libro/Película/Videojuego/Música) | Tabla `media_types` | **Tabla** (correcto) | Pueden crecer; el Admin los administra. Eliminar `MediaTypeConstants` muerto. |
| `Format`, `Platform`, `Genre`, `Creator` | Tablas | **Tabla** (correcto) | Catálogos administrados por Admin, con metadata (Genre.Description). |
| `Role` (rol de creador: Director/Autor/...) | Tabla `roles` + constantes `RoleConstants` | **Tabla** (correcto) | Extensible; renombrar a `CreatorRole` y eliminar `RoleConstants` muerto. |
| `Role` de Identity | Tabla `AspNetRoles` | **Tabla** | Mecanismo de Identity. |

`MediaTypeConstants` y `RoleConstants` están **sin uso** (código muerto) y sugieren erróneamente que son constantes. Eliminar.

---

## G. DTOs / Mappers / Validators

**DTOs:**
- `Rating`/`IsFavorite` en DTOs de Item: **eliminar** (no existen en `Item`; pertenecen a `UserItem`).
- Faltan DTOs de Biblioteca (se crean en F1): `PeticionAgregarABibliotecaDto`, `PeticionActualizarUserItemDto`, `RespuestaUserItemDto`, `FiltroBiblioteca`.
- `RespuestaItemDto` no incluye imágenes ni fechas; el frontend espera `mainImageUrl`, `createdAt`, `updatedAt`, `rating`, `isFavorite`. **Desajuste de contrato** con el frontend. Alinearlo (añadir `CreatedAt`/`UpdatedAt` y datos de imagen) o documentar el cambio.
- DTOs record con `init` — correcto.

**Mappers:**
- `ImagenMapper.MapToDto`: bug `Id = image.ItemId` → corregido (F5).
- Uso de mappers estáticos manuales: **correcto y KISS**. No hace falta AutoMapper. El "sobre-mapeo" no aplica porque las proyecciones son simples.
- Para listas de items se carga la entidad con `Include` y se mapea en memoria; aceptable para este volumen. Mejora opcional: proyección directa `Select` a DTO (ver sección H).

**Validators:**
- `ItemValidator` valida `Rating` de un DTO que no mapea a `Item` → validación muerta. Revisar tras eliminar `Rating`/`IsFavorite`.
- Faltan validators de Biblioteca (se crean en F1): rating 1–5, `ItemId` obligatorio, longitud de `Progress`/`Review`.
- `LoginDto` sin validator (menor).
- Los validators viven en Application — correcto. La validación de entrada (formato/longitud) está bien ubicada. Las reglas de negocio (no duplicar item, ownership, item eliminado) deben vivir en el servicio (Application) y reforzarse con constraints en BD.

---

## H. EF Core / SQL

**Consultas problemáticas:**
- `ItemService.ObtenerItemsPaginado` usa `Include("ItemGenres.Genre,ItemCreators.Creator")` → dos colecciones ⇒ **posible cartesian explosion**. Para items con muchos géneros/creadores multiplica filas. Usar `AsSplitQuery()` o proyección `Select` a DTO.
- `ObtenerItems` (L58) trae **todos** los items con includes, sin paginar ni limitar → payload completo en memoria. Debería limitarse o reutilizar el paginado.
- `FindAsync`/`GetFirstOrDefaultAsync` tienen overloads inconsistentes en tracking (uno siempre tracking, otro con `disableTracking`). Unificar firma con `disableTracking` explícito.
- No hay `AsNoTracking` en `GetFirstOrDefaultAsync` (string) por defecto — los servicios de catálogo actualizan a partir de entidades trackeadas (correcto), pero conviene ser explícito.

**Índices/constraints:**
- Correctos: `IX_user_items_UserId_ItemId` (unique), `IX_item_genres_ItemId_GenreId` (unique), `IX_item_creators_ItemId_CreatorId_RoleId` (unique), índice parcial único `IX_item_images_ItemId WHERE IsPrimary=true`, `EmailIndex`, `UserNameIndex`.
- Faltan: índice por `Title` para búsqueda (`LOWER(Title) LIKE` no usa índice → búsqueda full scan; a escala pequeña ok). Índice compuesto para filtrar biblioteca por `(UserId, Status)`.
- **Soft-delete vs unique indexes:** con `DeletedAt`, los índices únicos siguen bloqueando re-crear un item/género/relación borrado. Recomendación: índices únicos parciales `WHERE DeletedAt IS NULL` o hard-delete de agregados (ItemGenres/ItemCreators/ItemImages) al borrar el item.
- **Concurrencia:** no hay concurrency token. Las actualizaciones de `UserItem` (rating/progreso) son last-write-wins. Aceptable a este volumen; si se requiere, añadir `xmin` (Postgres) o un `Version`/`RowVersion`.

**AuditInterceptor:** correcto para CreatedAt/UpdatedAt. El soft-delete (Deleted→Modified) **exige** query filter (F9) y reevaluar cascade deletes: hoy `UserItem` tiene `Cascade` desde `Item`, pero con soft-delete nunca se dispara DELETE → los `UserItem` de items "borrados" persisten (correcto para no perder biblioteca, pero deben ocultarse en consultas junto con el item).

---

## I. API / Seguridad

**Autenticación:** JWT + Identity bien configurados (`AddIdentity`, `AddJwtBearer` con Issuer/Audience/Key). Claims incluyen `NameIdentifier`, `Email`, `Name`, roles. **Bien.**

**Autorización:**
- `[Authorize(Roles = "Admin")]` en escritura de items/catálogos: correcto.
- `GET` de items/catálogos `[AllowAnonymous]`: correcto para catálogo público.
- **Bug:** `ItemImagesController.SubirImagenChunk` sin `[Authorize]` ni `[HttpPost]` → subida anónima (F4). Además el contenedor Blob se crea con `PublicAccessType.Blob` (lectura pública) — revisar si es intencional.
- **Vulnerabilidad IDOR:** `ItemController.AgregarItem` extrae `userId` del claim (correcto). Los catálogos no manejan recursos de usuario (sin riesgo). **La biblioteca, al implementarse, debe usar SIEMPRE el userId del token** (así se hace en F1).

**API REST:**
- Verbos/status correctos en la mayoría (201 con `CreatedAtAction`, 204 en update/delete, 404 vía handler).
- `GlobalExceptionHandler` produce ProblemDetails estandarizados — excelente. **Falta** mapear `BusinessRuleException`→409 y `DbUpdateException` de unique index→409.
- Paginación expuesta vía header `X-Pagination` + registros en body; el `ProducesResponseType(typeof(RespuestaPaginada<T>))` no coincide con lo que se devuelve (body es `IEnumerable<T>`). Ajustar el anotado o devolver el objeto completo.
- `ItemImagesController` no tiene `[ApiVersion("1.0")]` (funciona por `AssumeDefaultVersionWhenUnspecified`, pero es inconsistente).

---

## J. Paginación

Endpoint con paginación: `GET /Item/paginado`, `GET /Genres/paginado`, `GET /MediaType/paginado`, `GET /Platform/paginado`, `GET /Creator/paginado`, `GET /Format/paginado` (+ `GET /Biblioteca/paginado` nuevo).

**Problemas:**
1. `PeticionPaginacion.PageSize` clampa a 60 como máximo pero permite 0/negativos → `Skip((page-1)*size)` con size=0 o page=0 lanza/retorna vacío. **Fijar mínimo 1** (F8).
2. `PageNumber` sin validación de mínimo (acepta 0/negativos).
3. Ordenación: `ObtenerPaginadosAsync` siempre hace `OrderByDescending(Id)` (Guid aleatorio → orden arbitrario, sin sentido de negocio) e **ignora** `FiltroItem.OrdenarPor`/`OrdenDescendente` que el frontend ya envía (F8).
4. `Take()` sin límite explícito adicional — ok por el clamp de 60, pero el `Count` se hace siempre (aceptable).
5. El orden actual (Guid) es **estable** (no rompe la paginación), pero no es determinista en sentido de negocio.

**Solución (F8):** clamp de `PageNumber>=1` y `PageSize` en [1,60]; soporte de `OrdenarPor`/`OrdenDescendente` en el repositorio mediante orden dinámico (whitelist de propiedades por entidad). Para el volumen esperado, offset pagination es suficiente; **no** hace falta keyset aún.

---

## K. SOLID / KISS

**Violaciones reales:**
- **SRP:** `ItemImageService` mezcla CRUD de imágenes + lógica de upload por chunks a Blob. Aceptable (misma raíz de agregado), no urgente.
- **DIP:** `Domain` depende de Identity (Infrastructure). Pragmático; documentar.
- **ISP:** `IGenericRepository<T>` está bien (pocos métodos). No es problema.
- **KISS / sobreingeniería:**
  - `UnitOfWork` con `Begin/Commit/RollbackTransaction` que **no se usan** en ningún servicio (código muerto). Mantener la interfaz de repositorios, pero los métodos de transacción pueden eliminarse o usarse (los servicios dependen de un único `SaveChanges`, que ya es transaccional).
  - `ServiceBus` integrado en `AgregarItem`: el evento se publica **después** del `SaveChanges`; si Azure falla, el item ya se guardó pero la petición devuelve 500 (inconsistencia). Además acopla una operación de catálogo a infraestructura externa. **Opción KISS:** mover a un try/catch con log, o eliminar la notificación si no hay consumidor real. Se documenta, no se elimina (puede ser un requisito de producto).
  - `MediaTypeConstants`/`RoleConstants`: muertos → eliminar.
  - Interfaces de servicio 1:1 con implementaciones: aceptables para DI y testabilidad, no son sobreingeniería.
  - GenericRepository: correcto para este tamaño; no es innecesario.
- **Open/Closed:** añadir un nuevo tipo de contenido no exige tocar lógica (los catálogos son tablas, `Metadata` es jsonb). Bien.

---

## L. Testing (propuesta)

**Unit tests (Application):**
1. `ItemService.ActualizarItem` sincroniza `GenreIds`/`CreatorIds` (F7).
2. `BibliotecaService`: no duplicar item (unique), ownership (userId de otro usuario → NotFound/Forbidden), rating fuera de 1–5 → error.
3. `UserItemMapper`: mapeo correcto de estado/fechas.
4. Paginación: pageNumber=0/pageSize=0 clampados.

**Integration tests (EF Core + Postgres):**
5. Soft-delete excluye items borrados de todas las consultas (F9).
6. Unique index `(UserId, ItemId)` rechaza duplicado.
7. Índice parcial `IsPrimary` rechaza dos imágenes primarias.
8. Crear item con géneros/creadores válidos persiste sin error de FK (F6).

**API tests:**
9. `POST /api/v1/Biblioteca` sin token → 401; con token de otro usuario → no puede tocar ajenos (IDOR).
10. `POST /api/v1/ItemImages/{itemId}` sin Admin → 403.
11. CRUD item: solo Admin; status 201/204/404.
12. Favoritos/rating solo sobre items propios de la biblioteca.

---

## M. Plan de refactor (implementado en este trabajo)

> Estado: **implementado y compilado con 0 errores / 0 warnings** (`dotnet build BibliotecaMultimedia.sln`, 2026-08-20). Cambios de comportamiento requieren BD disponible (no probados en ejecución por decisión de solo análisis estático; la migración generada no se ha aplicado).

**Fase 1 — Seguridad y bugs** ✅
- F4: `[HttpPost]` + ruta `{itemId:guid}` + `[Authorize(Roles="Admin")]` en `SubirImagenChunk`; sanitizar `fileName` (`SanitizarFileName`).
- F2: corregir `ActualizarImagenAsync` (tracking) y `EliminarImagenAsync` (llamar `Eliminar`).
- F3: `PlataformaService.EliminarPlataforma` con `SaveChanges`.
- F5: `ImagenMapper` `Id = image.Id`.
- F10: `BusinessRuleException`→409 en handler (además del `NotFoundException` del Domain).

**Fase 2 — Integridad de datos** ✅
- F6: validar existencia de FKs en item (media type, format, platform, géneros, creadores) — `ValidarReferenciasAsync`/`ValidarReferenciasActualizarAsync`; seed de catálogos `roles`/`media_types` (`DatabaseSeeder.SeedCatalogoAsync` llamado en `Program.cs`, idempotente); `RoleId` válido (rol "Autor" del catálogo).
- F9: query filter global `DeletedAt == null` en `AppDbContext.OnModelCreating` (solo entidades `BaseEntity`).
- F8: paginación mínima (PageNumber≥1, PageSize 1–60) y `OrdenarPor`/`OrdenDescendente` implementados en `GenericRepository` (orden por propiedad, seguro por default a `Id`).
- F7: `ActualizarItem` sincroniza géneros/creadores (`SincronizarRelaciones`).

**Fase 3 — Arquitectura** ✅
- F1: feature de Biblioteca completa (DTOs, mapper, validator, service, controller, DI, `ExpressionExtensions.And`). Ownership por `UserId` del JWT en todos los endpoints.
- Unificada la `NotFoundException` (se eliminó la duplicada de Application; todos los servicios usan la de `Domain.Exceptions`; handler con un solo branch).
- Renombrado `AppDbContext.Roles`→`CreatorRoles` (DbSet, `IUnitOfWork`, `UnitOfWork`, `DatabaseSeeder`, `ItemService`) — elimina el warning CS0114.

**Fase 4 — Performance** ✅
- `AsSplitQuery()` en consultas paginadas con includes de colecciones de `Item`.
- Proyección a DTO en consultas de listas: nuevo `ObtenerPaginadosProyectadosAsync<TResult>` en `GenericRepository`/`IGenericRepository` (selector SQL-translatable, sin materializar la entidad completa) + `ItemMapper.ProyeccionLista()`/`MapProyeccionToDto`; usado en `ItemService.ObtenerItemsPaginado`.

**Fase 5 — Limpieza y contrato** ✅
- Alineado el contrato con el frontend: `Item` ahora incluye `Descripcion`, `Rating` (0–10 en validator), `IsFavorite`, `IsbnOrUpc` como columnas reales (antes campos muertos en DTOs). `ItemMapper` expone `MainImageUrl` (imagen primaria o primera), `CreatedAt`/`UpdatedAt`. Frontend `item.model.ts` actualizado.
- Migración generada: `20260820182917_AgregarCamposItem` (añade 4 columnas a `items`; no aplicada por falta de BD).
- Transacciones del UnitOfWork: firmas con `CancellationToken` y **usadas** en `ItemService.AgregarItem` (`Begin`/`Commit`/`Rollback`).
- Paquetes vulnerables actualizados: eliminado `Microsoft.AspNetCore.Identity 2.3.10` (obsoleto) de Domain; `System.Security.Cryptography.Xml` 10.0.11; `Microsoft.AspNetCore.OpenApi` 10.0.11 + `Microsoft.OpenApi` 2.12.0 → warnings NU1903 eliminados.

**Pendiente / nota** ⏳
- Aplicar la migración `AgregarCamposItem` contra una BD real (no hay PostgreSQL disponible en este entorno).
- Los endpoints de catálogos usan ordenación/paginación por propiedad vía `OrdenarPor`; la proyección de listas de `Item` mantiene ese orden en la entidad antes del `Select`.

---

## Veredicto final

¿Este backend está correctamente diseñado para una plataforma donde administradores gestionan contenido multimedia y usuarios construyen su propia biblioteca personalizada?

**Arquitectónicamente sí es correcto y proporcionado** (N-Layers, DI, validación, manejo de errores, catálogos como tablas, enums bien elegidos). Pero **funcionalmente está incompleto** (falta la biblioteca) y contiene **bugs reales de persistencia y seguridad** (imágenes no-op, delete no persistido, subida anónima, soft-delete sin filtro, FK `Guid.Empty`). Tras aplicar los fixes F1–F10 descritos, la base queda sólida para crecer sin sobrecomplicarla.

**Regla aplicada:** Correctness > Security > Data Integrity > Maintainability > Performance > Architecture purity.