# Sistema de Reservas de Hoteles: bitácora del proceso y teoría

Documento de seguimiento del proyecto. Registra **qué se hizo**, **por qué se hizo así** y **qué buenas prácticas** respaldan cada decisión. Se actualiza a medida que avanza el trabajo.

## 1. Objetivo del proyecto

Construir una API de reservas de hotel con C# y .NET, simulando un entorno de trabajo profesional: tareas organizadas en Issues, desarrollo en ramas aisladas y cambios integrados mediante Pull Requests.

**Stack**

- C# y .NET 11 (ASP.NET Core Web API)
- Entity Framework Core 11 con SQL Server (LocalDB en desarrollo)
- Visual Studio 2026
- Git y GitHub (repositorio, Issues, Projects)

## 2. Backlog inicial

| Issue | Tema | Contenido |
|---|---|---|
| 1 | Entidades y persistencia | Modelos `User`, `Room`, `Reservation`; `DbContext` con EF Core; relaciones; primera migración |
| 2 | Concurrencia optimista | Token `RowVersion` en `Room`; captura de `DbUpdateConcurrencyException`; error limpio en la API |
| 3 | Aislamiento de transacciones | Endpoint de creación de reservas con `IDbContextTransaction` (cobro, registro y disponibilidad; ACID) |
| 4 | Motor de reglas de negocio | Tarifas dinámicas y penalizaciones por cancelación con Strategy o Chain of Responsibility |

Los Issues tienen dependencias: el 2 necesita las entidades del 1, y el 3 necesita ambos. Por eso se trabajan en orden.

## 3. Flujo de trabajo

1. Cada requerimiento es un **Issue** en GitHub, con criterios de aceptación.
2. El Issue se trabaja en una **rama propia** (`feature/issue-N-descripcion`). Nunca se trabaja directo en `main`.
3. Al terminar, se abre un **Pull Request** hacia `main`, donde se revisa el código antes de integrarlo.
4. El avance se refleja en un **tablero Kanban** (Backlog, To Do, In Progress, Code Review, Done).

---

## 4. Pasos realizados

### Paso 1: Crear el repositorio en GitHub

**Qué se hizo:** repositorio `reservas-hotel`, público, con README y `.gitignore` de plantilla *VisualStudio*.

**Teoría y buenas prácticas**

- **Nombre en minúsculas con guiones:** convención común de GitHub; evita problemas con URLs y con sistemas que distinguen mayúsculas.
- **README inicial:** es la portada del proyecto. Además, crea el primer commit en `main`, necesario para poder crear ramas y Pull Requests.
- **`.gitignore` de VisualStudio:** evita subir `bin/`, `obj/` y `.vs/`, que se regeneran al compilar, pesan y generan conflictos. Hacerlo desde el inicio es clave: un archivo que ya se subió queda en el historial aunque se borre después.
- **Repositorio público:** útil para portfolio, pero obliga a no subir nunca credenciales ni datos personales.
- **`main` como rama estable:** nadie commitea directo ahí. Más adelante se puede reforzar con una *branch protection rule*.

### Paso 2: Clonar el repositorio y configurar la identidad de Git

**Qué se hizo:** configuración de nombre y email en Git y clonado del repositorio con Visual Studio.

```powershell
git config --global user.name "Tu Nombre"
git config --global user.email "ID+usuario@users.noreply.github.com"
git clone https://github.com/TU-USUARIO/reservas-hotel.git
git status
git branch
```

**Teoría y buenas prácticas**

- **Identidad de Git:** cada commit queda firmado con nombre y email. Permite saber quién cambió qué y GitHub lo usa para atribuir contribuciones.
- **Email noreply:** en un repositorio público, el email del commit es visible para cualquiera. GitHub ofrece una dirección `noreply` (en *Settings → Emails*) para no exponer el correo personal. También conviene activar *Keep my email addresses private* y *Block command line pushes that expose my email*.
- **`git clone`:** descarga el repositorio completo con su historial y configura el remoto `origin`.
- **`git status` y `git branch`:** comandos de "mirar antes de actuar". Evitan commitear en la rama equivocada.
- **Visual Studio y la terminal:** el IDE trae Git integrado (ventana *Cambios de Git*) y una terminal propia (*Ver → Terminal*). Recomendación: usar comandos para aprender qué ocurre realmente, y la ventana gráfica para revisar los *diffs* antes de cada commit.

### Paso 3: Crear el tablero Kanban en GitHub Projects

**Qué se hizo:** proyecto con plantilla *Board*, con el campo **Status** configurado con cinco columnas: `Backlog`, `To Do`, `In Progress`, `Code Review`, `Done`. Vinculado al repositorio, con automatizaciones activadas.

**Teoría y buenas prácticas**

- **Kanban:** hace visible el flujo de trabajo. `Backlog` es lo no priorizado, `To Do` lo que se hará ahora, y `Code Review` representa la revisión del PR por otra persona.
- **Límite de trabajo en curso (WIP):** no más de una o dos tarjetas en `In Progress`. Terminar antes de empezar otra cosa evita ramas a medio hacer que se desactualizan.
- **Vincular el proyecto al repositorio:** permite agregar los Issues al tablero y ver su estado desde cada uno.
- **Automatizaciones (*Workflows*):** `Item closed → Done` y `Pull request merged → Done` mantienen el tablero actualizado sin depender de moverlo a mano.

### Paso 4: Crear los Issues del backlog

**Qué se hizo:** cuatro etiquetas (`feature`, `database`, `backend`, `architecture`) y los cuatro Issues descritos en la sección 2, cada uno con contexto, alcance y **criterios de aceptación**. Todos se agregaron al tablero; el Issue 1 pasó a `To Do`.

**Ejemplo de estructura de un Issue**

```markdown
## Contexto
Qué problema se resuelve y por qué.

## Alcance
- Qué incluye este Issue

## Criterios de aceptación
- [ ] Condición verificable 1
- [ ] Condición verificable 2
```

**Teoría y buenas prácticas**

- **Un Issue es una unidad de trabajo acotada:** describe qué hacer y cuándo está terminado.
- **Criterios de aceptación:** evitan el "¿ya está terminado?". Quien revisa el PR sabe exactamente qué verificar.
- **Issues pequeños y enfocados:** si un Issue es demasiado grande, la rama dura semanas y el PR es imposible de revisar.
- **Etiquetas:** permiten filtrar y priorizar de un vistazo.
- **Dependencias:** solo el Issue sin dependencias pendientes pasa a `To Do`.

### Paso 5: Crear la rama de trabajo

**Qué se hizo:** rama `feature/issue-1-modelos-bd` a partir de `main` actualizado. La tarjeta del Issue 1 pasó a `In Progress`.

```powershell
git checkout main
git pull origin main
git checkout -b feature/issue-1-modelos-bd
git branch
```

**Teoría y buenas prácticas**

- **Partir de `main` actualizado:** una rama que nace de una base vieja genera conflictos al integrarse.
- **`git checkout -b`:** crea la rama y se mueve a ella. Todo lo que se haga queda aislado de `main`, que sigue estable.
- **Convención de nombre `tipo/issue-N-descripcion`:** el tipo (`feature/`, `fix/`), el número del Issue (trazabilidad) y una descripción corta en minúsculas con guiones. Se entiende de qué trata la rama sin abrirla.
- **Autocompletar con Tab:** evita errores de tipeo al escribir nombres de rama.

### Paso 6: Crear la solución .NET y subir el primer commit

**Qué se hizo:** plantilla **ASP.NET Core Web API** con solución `ReservasHotel` y proyecto `ReservasHotel.Api`, con la solución y el proyecto en carpetas separadas. Primer commit y primer push de la rama.

```powershell
git status
git add .
git commit -m "feat: crear solución y proyecto Web API base (#1)"
git push -u origin feature/issue-1-modelos-bd
```

**Teoría y buenas prácticas**

- **Solución en la raíz y proyecto en subcarpeta:** estructura estándar de .NET. Permite sumar más proyectos (por ejemplo, de pruebas) sin reorganizar.
- **Compilar antes de commitear:** nunca se sube código que no compila.
- **`git status` antes de `git add .`:** verifica que el `.gitignore` funciona (sin `bin/`, `obj/` ni `.vs/`).
- **Conventional Commits:** formato `tipo: descripción (#issue)`. Tipos usuales: `feat`, `fix`, `docs`, `refactor`, `chore`. El `(#1)` enlaza el commit con el Issue.
- **Commits pequeños y frecuentes:** facilitan la revisión y permiten deshacer solo una parte.
- **`git push -u`:** sube la rama por primera vez y configura el seguimiento, de modo que después alcanza con `git push`.

#### Problemas encontrados y cómo se resolvieron

**1. `error: src refspec ... does not match any`**

- **Causa:** un error de tipeo al crear la rama (`modelados` en vez de `modelos`). El comando de push buscaba un nombre que no existía.
- **Diagnóstico:** `git branch`, `git status` y `git log --oneline` mostraron la rama con el nombre mal escrito y el commit ya hecho.
- **Solución:** renombrar la rama local, que todavía no se había subido.

```powershell
git branch -m feature/issue-1-modelos-bd
git push -u origin feature/issue-1-modelos-bd
```

- **Lección:** `git branch -m` es seguro mientras la rama no esté en el remoto. Si ya se hubiera subido, habría que borrar también la rama remota.

**2. Email personal expuesto en el historial**

- **Causa:** el commit se hizo con el email personal, y el repositorio es público.
- **Solución:** configurar el email `noreply` y reescribir el autor del último commit.

```powershell
git config --global user.email "ID+usuario@users.noreply.github.com"
git commit --amend --reset-author --no-edit
git push --force-with-lease
```

- **Teoría:** reescribir un commit cambia su hash, por eso el push requiere forzado. `--force-with-lease` es la variante segura: solo pisa el remoto si está exactamente como se lo dejó, y no borra trabajo ajeno por accidente. Es aceptable únicamente en ramas propias; en `main` o en ramas compartidas no se hace.
- **Detalle:** el email del ejemplo (`ID+usuario@...`) es un formato; hay que usar la dirección real con el número de ID de GitHub. Si el email no corresponde a la cuenta, GitHub no atribuye el commit al perfil.
- **Limitación:** el commit viejo puede seguir accesible por su hash directo durante un tiempo.

### Paso 7: Limpiar la plantilla y crear las entidades

**Qué se hizo:** commit de limpieza que elimina los archivos de ejemplo de la plantilla, y creación de las entidades en la carpeta `Entities`.

```powershell
git rm ReservasHotel.Api/Controllers/WeatherForecastController.cs ReservasHotel.Api/WeatherForecast.cs
git commit -m "chore: eliminar ejemplo WeatherForecast de la plantilla (#1)"
```

**Entidades**

```csharp
namespace ReservasHotel.Api.Entities;

public enum RoomType { Single = 1, Double = 2, Suite = 3 }

public enum ReservationStatus { Pending = 1, Confirmed = 2, Cancelled = 3 }

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public decimal PricePerNight { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

public class Reservation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public ReservationStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
}
```

(En el proyecto cada tipo está en su propio archivo.)

**Teoría y buenas prácticas**

- **Un archivo por clase, en una carpeta `Entities`:** convención de C#; facilita encontrar cada cosa.
- **Commit de limpieza separado:** mantiene un historial legible, sin mezclar mantenimiento con funcionalidad.
- **Enums en lugar de strings:** el compilador impide valores inválidos y los typos.
- **`decimal` para dinero:** `double` y `float` generan errores de redondeo por su representación binaria (`0.1 + 0.2` no da exactamente `0.3`).
- **`DateOnly` para fechas de estadía:** un hotel reserva noches, no instantes. `DateTime` arrastra hora y zona horaria, que solo generan bugs.
- **Clave foránea más propiedad de navegación (`UserId` y `User`):** la primera es lo que se guarda en la base; la segunda permite navegar en el código (`reservation.User.Name`).
- **Colecciones inicializadas:** representan el lado "uno" de una relación uno-a-muchos y evitan `NullReferenceException`.
- **`= string.Empty` y `= null!`:** con *nullable reference types* habilitados, el primero da un valor seguro por defecto y el segundo indica que EF Core cargará la navegación.
- **Sin `RowVersion` todavía:** pertenece al Issue 2; cada Issue se mantiene enfocado para que el PR sea revisable.

### Reinicio del proyecto de código

Después de los pasos 1 a 7 (y parte del 8), el proyecto de código se rehízo desde cero conservando el repositorio, los documentos y el README. **No se reescribió el historial de Git**: se siguió trabajando en la misma rama y se commiteó el nuevo estado encima.

**Qué cambió respecto a la primera versión**

- Las entidades pasaron de la raíz de la solución a `ReservasHotel.Api/Entities`.
- La solución (`.slnx`) ahora incluye el proyecto. En el primer commit estaba vacía (`<Solution />`).
- El proyecto pasó de `net10.0` a `net11.0`, con todos los paquetes en `11.0.0`.
- Se agregó `Data/AppDbContext.cs`.

**Problemas encontrados y cómo se resolvieron**

1. **Typo en un nombre de archivo (`Usser.cs`).** Se corrigió con `Rename-Item`. En C# la convención es que el archivo se llame como su clase.
2. **Los ejemplos de la plantilla reaparecieron** (`WeatherForecast` y su controlador) al regenerar el proyecto. Se borraron de nuevo.
3. **Faltaba registrar el `DbContext` en `Program.cs`.** El indicio fue que `git status` no mostraba `Program.cs` como modificado. Lección: `git status` también sirve para detectar lo que *falta* haber cambiado.
4. **Resto de Visual Studio en el `.csproj`:** un bloque `<Folder Include="Data" />` que aparece al crear una carpeta vacía y sobra cuando ya tiene archivos.
5. **Ruido de la plantilla en `launchSettings.json` y `.http`:** Visual Studio asigna puertos al azar cada vez que genera un proyecto. Se descartaron con `git restore` para que el diff muestre solo cambios reales.
6. **Paginador de Git trabado (`(END)`).** Se sale con `q`. Para evitarlo: `git --no-pager <comando>` o `git config --global core.pager cat`.

**Cómo se ordenaron los cambios**

En lugar de un único `git add .`, se hicieron tres commits por tema:

```powershell
git add -A -- ReservasHotel.Api/Entities Reservation.cs ReservationStatus.cs Room.cs RoomType.cs User.cs
git commit -m "refactor: mover entidades a ReservasHotel.Api/Entities (#1)"

git add ReservasHotel.slnx ReservasHotel.Api/ReservasHotel.Api.csproj
git commit -m "chore: agregar proyecto a la solución y paquetes de EF Core (#1)"

git add ReservasHotel.Api/Data ReservasHotel.Api/appsettings.json ReservasHotel.Api/Program.cs
git commit -m "feat: configurar DbContext con relaciones y cadena de conexión (#1)"
```

**Teoría y buenas prácticas**

- **Revisar el `git diff` antes de commitear:** permite separar los cambios reales del ruido generado.
- **No usar `git add .` cuando hay ruido en el árbol de trabajo:** mezclaría todo en un commit.
- **Commits por tema:** si algo falla se revisa o revierte una sola parte, y quien revise el PR entiende la historia.
- **`git add -A` con rutas explícitas:** agrega archivos nuevos y también registra los borrados del lugar viejo, de modo que Git puede mostrar el cambio como un movimiento de archivos.
- **No reescribir historial sin necesidad:** rehacer el código encima del historial existente es más seguro que forzar pushes.
- **Una sola versión de .NET en toda la solución:** el framework, los paquetes de EF Core y la herramienta `dotnet-ef` deben coincidir. .NET 10 es LTS (soporte más largo); .NET 11 es la versión más reciente.

### Paso 8: Entity Framework Core y `DbContext`

**Qué se hizo:** instalar los paquetes de EF Core, crear el `DbContext` con las relaciones y registrarlo con una cadena de conexión a LocalDB.

```powershell
dotnet add ReservasHotel.Api package Microsoft.EntityFrameworkCore.SqlServer
dotnet add ReservasHotel.Api package Microsoft.EntityFrameworkCore.Design
```

**`Data/AppDbContext.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using ReservasHotel.Api.Entities;

namespace ReservasHotel.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(r => r.Number).IsRequired().HasMaxLength(10);
            entity.HasIndex(r => r.Number).IsUnique();
            entity.Property(r => r.PricePerNight).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.Property(r => r.TotalPrice).HasPrecision(18, 2);

            entity.HasOne(r => r.User)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Room)
                  .WithMany(room => room.Reservations)
                  .HasForeignKey(r => r.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
```

**Cadena de conexión (`appsettings.json`)**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ReservasHotel;Trusted_Connection=True;TrustServerCertificate=True"
}
```

**Registro en `Program.cs`**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Teoría y buenas prácticas**

- **Proveedor de base de datos:** EF Core es independiente de la base; el paquete `...SqlServer` traduce C# a SQL para SQL Server. `...Design` aporta las herramientas de tiempo de diseño (migraciones). Los paquetes de EF Core deben mantenerse en la misma versión.
- **Commit aparte para las dependencias:** son un cambio distinto del código y se pueden revisar o revertir por separado.
- **Constructor con `DbContextOptions`:** la configuración viene de afuera, así el contexto no depende de dónde está la base y es más fácil de probar.
- **`DbSet<T>`:** representa una tabla y es la puerta de entrada para consultar y guardar.
- **Fluent API en lugar de atributos:** deja las entidades como clases simples y concentra las reglas de la base en un solo lugar.
- **`IsRequired` y `HasMaxLength`:** sin ellos, EF crea columnas `nvarchar(max)` que aceptan nulos.
- **Índices únicos (`Email`, `Number`):** la base misma garantiza la unicidad, incluso si falla la validación del código.
- **`HasPrecision(18, 2)`:** define los decimales de los montos y evita el truncamiento por precisión por defecto.
- **`DeleteBehavior.Restrict`:** impide borrar un usuario o habitación con reservas. El borrado en cascada eliminaría el historial en silencio, algo peligroso en un sistema con pagos.
- **Inyección de dependencias (`AddDbContext`):** registra el contexto con vida por petición HTTP, la que corresponde para EF Core.
- **LocalDB con autenticación de Windows:** no hay usuario ni contraseña en el archivo, por lo que es seguro de commitear. Una cadena con contraseña nunca se sube al repositorio: se usa `dotnet user-secrets` o variables de entorno.

**Resultado**

- [x] Paquetes de EF Core instalados (versión `11.0.0`)
- [x] `AppDbContext` creado con relaciones, índices y precisión de montos
- [x] Cadena de conexión agregada y contexto registrado en `Program.cs`
- [x] Cambios commiteados y subidos (ver los tres commits del reinicio)

### Paso 9: Migración inicial y creación de la base de datos

**Qué se hizo:** instalación de la herramienta `dotnet-ef`, generación de la migración `InitialCreate`, revisión del código generado, y commit y push de la carpeta `Migrations`.

```powershell
dotnet tool install --global dotnet-ef
dotnet ef --version
cd ReservasHotel.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Teoría y buenas prácticas**

- **Qué es una migración:** un archivo de código que describe un cambio de esquema (crear tablas, columnas, índices, relaciones). EF Core la genera comparando el modelo actual con el estado anterior.
- **`dotnet-ef`:** herramienta de línea de comandos de EF Core, global (se instala una vez por computadora). Su versión debe coincidir con la de los paquetes del proyecto.
- **Archivos generados en `Migrations/`:**
  - `..._InitialCreate.cs`: métodos `Up` (aplicar el cambio) y `Down` (deshacerlo).
  - `..._InitialCreate.Designer.cs`: metadatos internos de EF.
  - `AppDbContextModelSnapshot.cs`: una "foto" del modelo actual; la próxima migración se calcula contra ella.
- **`migrations add` solo genera el archivo; `database update` es el que ejecuta el SQL.** También crea la tabla `__EFMigrationsHistory`, donde EF anota qué migraciones ya se aplicaron.
- **Revisar la migración antes de aplicarla:** es código generado, pero modifica la base, así que el responsable de que haga lo correcto es quien la genera. Se verificó que `Up` contuviera:
  - `CreateTable` de `Rooms`, `Users` y `Reservations`
  - `Name` (100) y `Email` (200) con largo máximo y no nulos; `Number` con largo 10
  - Montos como `decimal(18,2)` y fechas como `date`
  - Índices únicos en `Email` y `Number`
  - Dos claves foráneas en `Reservations` con `ReferentialAction.Restrict`
- **`Down` borra primero `Reservations`:** no se puede eliminar una tabla a la que otra todavía apunta con una clave foránea.
- **Índices en claves foráneas:** EF los agrega solos (`IX_Reservations_RoomId` y `IX_Reservations_UserId`) porque aceleran los *joins* y las comprobaciones de integridad.
- **`Type` y `Status` se guardan como `int`:** en la base se ven `1`, `2`, `3` en lugar de los nombres de los enums.
- **Deshacer una migración:** `dotnet ef migrations remove` funciona solo si todavía **no** se aplicó a la base. Se usó una vez para regenerarla tras dudar de su contenido, sin consecuencias porque la base no se había tocado.
- **No editar a mano una migración ya aplicada ni el código generado:** si hay un error, se crea una migración nueva que lo corrija. Editar una vieja desincroniza la base propia de la de los demás.
- **Las migraciones se commitean al repositorio:** son parte del código. Cualquiera que clone el proyecto puede recrear la base con `dotnet ef database update`.
- **Una migración por cambio lógico, con nombre descriptivo:** `InitialCreate` ahora, y después, por ejemplo, `AddRowVersionToRoom` (Issue 2).

```powershell
git add ReservasHotel.Api/Migrations
git commit -m "feat: agregar migración InitialCreate (#1)"
git push
```

**Lo que esta migración todavía no controla:** a nivel de base nada impide una reserva con `CheckOut` anterior a `CheckIn`, ni dos reservas solapadas en la misma habitación. Es intencional: son reglas de negocio y de concurrencia que corresponden a los Issues 2, 3 y 4.

**Pendiente de este paso**

- [ ] Confirmar con `dotnet ef database update` que la base está creada
- [ ] Verificar `Users`, `Rooms`, `Reservations` y `__EFMigrationsHistory` en el Explorador de objetos de SQL Server

### Paso 10: Pull Request del Issue 1 (en curso)

**Qué se hace:** abrir un Pull Request de `feature/issue-1-modelos-bd` hacia `main`, autorrevisarlo, integrarlo y limpiar.

**Título y descripción sugeridos**

```markdown
feat: entidades y persistencia con EF Core (#1)

Closes #1

## Qué se hizo
- Entidades `User`, `Room` y `Reservation` con sus enums
- `AppDbContext` con relaciones, índices únicos y precisión de montos (Fluent API)
- Cadena de conexión a LocalDB y registro del contexto en `Program.cs`
- Migración `InitialCreate`

## Cómo probarlo
1. Clonar la rama
2. `dotnet ef database update` desde `ReservasHotel.Api`
3. Verificar las tablas `Users`, `Rooms` y `Reservations` en LocalDB

## Criterios de aceptación
- [ ] Las tres entidades existen con sus propiedades y relaciones
- [ ] El `DbContext` está registrado en `Program.cs`
- [ ] La migración `InitialCreate` se genera y se aplica sin errores
- [ ] Las tablas se crean correctamente en la base de datos
```

**Teoría y buenas prácticas**

- **`Closes #1`:** GitHub cierra el Issue cuando el PR se integra y, con el workflow del tablero, la tarjeta pasa a `Done`. Además enlaza la tarea con el código que la resolvió.
- **Descripción con "cómo probarlo":** quien revisa necesita poder reproducir el trabajo sin preguntar. Es lo que distingue un PR fácil de aprobar de uno que se estanca.
- **Autorrevisión en *Files changed*:** leer el propio diff antes de pedir revisión detecta la mayoría de los errores (archivos que no corresponden, restos de la plantilla, puertos de `launchSettings.json`).
- **Checklist de criterios:** se marcan solo los que de verdad se cumplen.
- **Modos de integración:**

| Modo | Qué hace | Cuándo conviene |
|---|---|---|
| **Squash and merge** | Junta todos los commits de la rama en uno solo en `main` | Historial de `main` limpio, un commit por Issue |
| **Create a merge commit** | Conserva todos los commits y agrega uno de unión | Cuando los commits intermedios tienen valor propio |
| **Rebase and merge** | Reaplica los commits sobre `main`, sin commit de unión | Historial lineal conservando cada commit |

  Para este proyecto se recomienda **Squash and merge**, con el título del PR (en formato Conventional Commits) como mensaje. El detalle fino queda visible en el PR.
- **Limpieza posterior:** borrar la rama en GitHub y actualizar `main` local antes de empezar el siguiente Issue, para que su rama nazca de un `main` que ya incluya este trabajo.

```powershell
git checkout main
git pull origin main
git branch -d feature/issue-1-modelos-bd
```

  Si `-d` se queja de que la rama no está integrada (squash reescribe los commits), y ya se verificó en GitHub que el PR está integrado, es seguro usar `-D`.

**Pendiente de este paso**

- [ ] Abrir el PR y revisarlo en *Files changed*
- [ ] Integrarlo (squash), borrar la rama y actualizar `main` local
- [ ] Verificar que el Issue 1 se cerró y la tarjeta quedó en `Done`

---

## 5. Glosario rápido de Git

| Comando | Para qué sirve |
|---|---|
| `git status` | Ver rama actual y cambios pendientes |
| `git branch` | Listar ramas (el `*` marca la activa) |
| `git checkout -b <rama>` | Crear una rama y moverse a ella |
| `git branch -m <nombre>` | Renombrar la rama actual |
| `git add .` | Preparar todos los cambios para el commit |
| `git commit -m "<mensaje>"` | Guardar los cambios preparados |
| `git commit --amend` | Modificar el último commit |
| `git push -u origin <rama>` | Subir una rama por primera vez |
| `git push --force-with-lease` | Forzar el push de forma segura (solo ramas propias) |
| `git rm <archivo>` | Borrar un archivo y registrar el cambio |
| `git log --oneline` | Ver el historial resumido |
| `git --no-pager show --stat HEAD` | Ver los archivos del último commit |
| `git --no-pager diff <archivo>` | Ver los cambios de un archivo sin que se trabe el paginador |
| `git add -A -- <rutas>` | Preparar altas, cambios y borrados de rutas concretas |
| `git restore <archivo>` | Descartar cambios sin commitear (irreversible) |
| `git branch -d <rama>` | Borrar una rama local ya integrada |
| `git config --global core.pager cat` | Desactivar el paginador de Git |

### Comandos de Entity Framework Core

| Comando | Para qué sirve |
|---|---|
| `dotnet tool install --global dotnet-ef` | Instalar la herramienta de EF Core |
| `dotnet ef migrations add <Nombre>` | Generar una migración nueva |
| `dotnet ef migrations remove` | Deshacer la última migración (solo si no se aplicó) |
| `dotnet ef database update` | Aplicar las migraciones pendientes a la base |

## 6. Próximos pasos

1. Confirmar la creación de la base con `dotnet ef database update` y verificar las tablas.
2. Cerrar el **Issue 1**: Pull Request, merge, limpieza de la rama y actualización de `main`.
3. **Issue 2:** concurrencia optimista con `RowVersion` (nueva rama desde `main`).
4. **Issue 3:** transacciones explícitas en la creación de reservas.
5. **Issue 4:** motor de reglas de negocio con Strategy o Chain of Responsibility.
