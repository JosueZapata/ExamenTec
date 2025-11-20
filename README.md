## ExamenTec

Suite full stack compuesta por un backend en ASP.NET Core 9 y un frontend Angular 19 para administrar categorías, productos y autenticación basada en roles.

---

### Requerimientos previos
- `.NET SDK 9.0` (incluye `dotnet` CLI)  
- `Node.js 22.14.0` y `npm 10.x` (Angular CLI 19 está validado sobre esta versión LTS)  
- `Angular CLI 19` instalada globalmente (`npm install -g @angular/cli@19`)  
- Postman v10+ si vas a consumir la colección incluida en `postman/`.

---

### Cómo correr backend y frontend

**Backend (`/back/ExamenTec.Api`)**
1. `cd back/ExamenTec.Api`
2. `dotnet restore`
3. `dotnet run --launch-profile https` (expone `https://localhost:7062` y `http://localhost:5171`)
   - *Alternativa*: abre la solución `ExamenTec.sln` en Visual Studio y ejecuta el perfil **https**.
4. API disponible en `https://localhost:7062` con Swagger en `/swagger`. Usa cualquiera de los métodos anteriores para asegurar los mismos puertos.

**Frontend (`/front`)**
1. `cd front`
2. `npm install`
3. `npm start` (alias de `ng serve`)
4. UI disponible en `http://localhost:4200`.

> Tip: importa `postman/ExamenTec_API.postman_collection.json` y `postman/ExamenTec_Local.postman_environment.json`, ejecuta el request `Auth > Login` y reutiliza el token para el resto de pruebas.

**Credenciales demo**
- `admin@examentec.com` / `Admin123!`
- `product@examentec.com` / `Product123!`
- `category@examentec.com` / `Category123!`

---

### Cómo correr pruebas automatizadas

**Backend**
1. `cd back`
2. `dotnet test ExamenTec.Tests/ExamenTec.Tests.csproj`
   - Ejecuta los unit tests de features, validadores y controladores usando el mismo entorno InMemory.

**Frontend**
1. `cd front`
2. `npm test` (abre Karma con Chrome) o `npm run test:ci` para modo headless.
   - Asegúrate de tener Node 22.14.0 y dependencias instaladas (`npm install`).

---

### Tecnología utilizada
- **Backend**: ASP.NET Core 9, EF Core InMemory, MediatR, FluentValidation, Mapster, JWT Bearer Auth, Swagger/Swashbuckle.
- **Frontend**: Angular 19, RxJS 7.8, Tailwind CSS, Karma/Jasmine, OpenAPI Generator para clientes de API.
- **Infraestructura**: Clean Architecture (Domain/Application/Infrastructure/Api) con enfoque Vertical Slice para cada feature, logging personalizado a base de datos, seeders automáticos para datos demo.

---

### Arquitectura seleccionada y razones
- **Clean Architecture por capas**: separa dominios (`ExamenTec.Domain`), reglas de aplicación (`ExamenTec.Application`), infraestructura (`ExamenTec.Infrastructure`) y entrega (`ExamenTec.Api`). Facilita pruebas unitarias y reemplazo de infraestructura sin tocar lógica de negocio.
- **CQRS ligero con MediatR**: los controladores delegan en Queries/Commands MediatR, lo que mantiene los endpoints delgados y centraliza validaciones, permitiendo pipeline behaviors como `ValidationBehavior`.
- **Frontend modular**: Angular organiza componentes por dominio (`components/`, `services/`, `interceptors/`), habilitando lazy features y mantenibilidad. Las rutas consumen servicios generados a partir del swagger, reduciendo desalineaciones.
- **Configuración InMemory**: la aplicación está pensada para usarse con EF Core InMemory, asegurando datos seed reproducibles sin dependencias externas.
- **Repositorio + Unit of Work**: `Infrastructure` implementa repositorios específicos (`CategoryRepository`, `ProductRepository`, etc.) coordinados por `UnitOfWork` para mantener transacciones coherentes y aislar el acceso a datos.

---

### Decisiones técnicas clave
- **Autenticación y autorización JWT**: tokens firmados con `Jwt:Key`, políticas `ProductAccess` y `CategoryAccess` y roles Admin/Product/Category para garantizar acceso granular desde backend y guardias en frontend.
- **Validaciones desacopladas**: FluentValidation + pipeline behavior aseguran que cada comando/query valide DTOs antes de tocar repositorios, reduciendo código repetido en controladores.
- **Mapster para mapeos**: evita manual mapping al convertir entidades EF a DTOs, simplificando respuestas enriquecidas (`CategoryName`, `StoreName`).
- **Seeder y logging persistente**: `DataSeeder` crea datos demo (categorías, productos, usuarios), mientras `DbLoggerProvider` centraliza logs en memoria para auditoría ligera.
- **OpenAPI-driven frontend**: script `npm run openapi` genera clientes Typescript al vuelo desde Swagger (`http://localhost:5171/swagger/v1/swagger.json`), garantizando contratos sincronizados.
- **CORS estricto por origen**: la política `ExamenTecApi` solo permite `http://localhost:4200`, restringiendo el intercambio de datos al frontend oficial y mitigando solicitudes no autorizadas.

---

### Funcionalidades y validaciones por endpoint

- **Autenticación**
  - `POST /api/v1/Account/login`
    - Genera un JWT con roles y expiración para sesiones en frontend.
    - Validaciones (FluentValidation): `Email` obligatorio, formato válido y ≤255 caracteres; `Password` obligatoria con mínimo 6 caracteres.

- **Categorías** (`Authorize` con política `CategoryAccess`, salvo `GET /search` que acepta roles `Admin,Product,Category`)
  - `GET /api/v1/Categories/search`
    - Busca nombres por término parcial y limita el resultado con `maxResults` (default 20) para autocompletado.
  - `GET /api/v1/Categories`
    - Devuelve listado paginado y filtrable por `searchTerm`.
  - `GET /api/v1/Categories/{id}`
    - Obtiene el detalle enriquecido de una categoría específica.
  - `POST /api/v1/Categories`
    - Crea una categoría nueva.
    - Validaciones: `Name` obligatorio y ≤200 caracteres; `Description` opcional pero ≤500 caracteres.
  - `PUT /api/v1/Categories/{id}`
    - Actualiza nombre/descrición manteniendo la misma categoría.
    - Validaciones idénticas al alta (nombre requerido ≤200, descripción opcional ≤500).
  - `DELETE /api/v1/Categories/{id}`
    - Elimina la categoría y devuelve `204 No Content` si tiene éxito.

- **Productos** (`Authorize` con política `ProductAccess`)
  - `GET /api/v1/Products`
    - Lista paginada con filtros por nombre vía `searchTerm`.
  - `GET /api/v1/Products/{id}`
    - Detalle completo incluyendo datos de categoría asociada.
  - `POST /api/v1/Products`
    - Registra un producto nuevo.
    - Validaciones: `Name` obligatorio ≤200 caracteres; `Description` opcional ≤500; `Price` y `Stock` deben ser ≥0; `CategoryId` obligatorio.
  - `PUT /api/v1/Products/{id}`
    - Actualiza un producto existente.
    - Validaciones replican las del alta (nombre requerido ≤200, descripción opcional ≤500, precio/stock ≥0, `CategoryId` obligatorio).
  - `DELETE /api/v1/Products/{id}`
    - Elimina el recurso y responde con `204 No Content`.

- **Logs**
  - `GET /api/v1/Logs`
    - Expone las entradas almacenadas por `DbLogger` para auditoría y debugging; no requiere payload ni validaciones adicionales.

---

### Estructura rápida del repositorio
- `back/` – solución .NET (`ExamenTec.Api`, `Application`, `Domain`, `Infrastructure`, `Tests`).docume
- `front/` – cliente Angular y assets.
- `postman/` – colección y ambiente para probar la API.

---

