# 🚀 TravelRequests API

API RESTful para gestión de solicitudes de viajes corporativos desarrollada con .NET 10 y Clean Architecture.

## 📋 Descripción

Sistema que permite a una empresa gestionar solicitudes de viajes corporativos con:
- Registro y autenticación de usuarios con JWT
- Roles: Solicitante y Aprobador
- Solicitudes de viaje
- Recuperación de contraseña con código de verificación

## Arquitectura

```
TravelRequests/
├── TravelRequest/              # API Layer (Controllers, Middleware)
├── TravelRequests.Application/ # Application Layer (Services, DTOs, Validators)
├── TravelRequests.Domain/      # Domain Layer (Entities, Interfaces, Enums)
├── TravelRequests.Infrastructure/ # Infrastructure Layer (Repositories, DbContext)
└── TravelRequests.Tests/       # Unit Tests (xUnit, Moq)
```

## Tecnologías

- .NET 10
- Entity Framework Core
- SQL Server
- JWT Authentication
- FluentValidation
- Swagger/OpenAPI
- xUnit + Moq
- Docker

## Como Correr El Proyecto

### 1. Clonar repositorio

```bash
git clone https://github.com/jesusvp20/PRUEBA-TECNICA-CONSWARE-.NETSistema-de-Gesti-n-de-Solicitudes-de-Viajes-Corporativos.git
cd TravelRequest
```

### 2. Configurar base de datos

**Opción A: Usar migraciones**
```bash
dotnet ef database update --project TravelRequests.Infrastructure --startup-project TravelRequest
```

**Opción B: Ejecutar script SQL**
```sql
-- Abrir SQL Server Management Studio
-- Ejecutar: Scripts/database.sql
```

### 3. Configurar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TravelRequestDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "TuClaveSecretaSuperSeguraDeAlMenos32Caracteres!",
    "Issuer": "TravelRequestsAPI",
    "Audience": "TravelRequestsClients",
    "ExpiresMinutes": 60
  }
}
```

### 4. Ejecutar aplicación

```bash
cd TravelRequest
dotnet run
```

Abrir: `https://localhost:5231/swagger`

##  Docker

```bash
# Construir imagen
docker build -t travelrequests-api -f TravelRequest/Dockerfile .

# Ejecutar contenedor
docker run -d -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Development --name travelrequests-api travelrequests-api

# Acceder
http://localhost:5000/swagger

```
> **Nota:** Los endpoints que requieren base de datos no funcionarán en Docker porque el contenedor no tiene conexión a SQL Server. Para producción, usar `docker-compose` con SQL Server incluido.

##  Tests

```bash
dotnet test TravelRequests.Tests
```

**Cobertura:** 55 tests unitarios
- UsuariosServiceTests (13 tests)
- SolicitudesViajeServiceTests (11 tests)
- ValidatorsTests (31 tests)

## Endpoints

**Auth:**
- `POST /api/auth/login` - Iniciar sesión

**Usuarios:**
- `POST /api/usuarios/registrar` - Registrar usuario
- `GET /api/usuarios` - Listar usuarios (solo Aprobador)

**Recuperación Contraseña:**
- `POST /api/recuperacioncontraseña/solicitar` - Solicitar código
- `POST /api/recuperacioncontraseña/restablecer` - Restablecer contraseña

**Solicitudes de Viaje:**
- `POST /api/solicitudesviaje` - Crear solicitud
- `GET /api/solicitudesviaje/mis-solicitudes` - Mis solicitudes
- `GET /api/solicitudesviaje` - Todas las solicitudes (solo Aprobador)
- `PATCH /api/solicitudesviaje/{id}/estado` - Cambiar estado (solo Aprobador)

> **Documentación completa:** Disponible en Swagger UI al ejecutar la aplicación (`/swagger`)

## 🔐 Autenticación

1. Registrar usuario en `/api/usuarios/registrar`
2. Login en `/api/auth/login` → obtener token JWT
3. En Swagger: Click "Authorize" → pegar solo el token
4. Acceder a endpoints protegidos

## Autor

**Jesús Vega** - [@jesusvp20](https://github.com/jesusvp20)


