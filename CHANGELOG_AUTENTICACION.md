# 📝 CHANGELOG - Implementación de Autenticación JWT

**Fecha:** 2024-11-29  
**Rama:** autenticación  
**Autor:** Asistente IA

---

## ✅ Cambios Realizados

### 1. Configuración JWT (`appsettings.json`)
- Añadida sección `Jwt` con:
  - `Key`: Clave secreta para firmar tokens (mínimo 32 caracteres)
  - `Issuer`: Emisor del token
  - `Audience`: Audiencia del token
  - `ExpiresMinutes`: Tiempo de expiración (60 min)

### 2. Autenticación en `Program.cs`
- Configurado middleware JWT Bearer
- Validación de: Issuer, Audience, Lifetime, SigningKey
- Políticas de autorización: `SoloAprobador`, `SoloSolicitante`
- Orden correcto: `UseAuthentication()` antes de `UseAuthorization()`

### 3. Nuevo `AuthController.cs`
Endpoints implementados:
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/registrar` | Registra usuario con contraseña hasheada |
| POST | `/api/auth/login` | Retorna token JWT con rol incluido |
| POST | `/api/auth/olvidar-contraseña` | Genera código 6 dígitos (5 min) |
| POST | `/api/auth/restablecer-contraseña` | Cambia contraseña con código válido |
| GET | `/api/auth/usuarios` | Lista usuarios (solo Aprobadores) |
| GET | `/api/auth/perfil` | Verifica token y muestra datos |

### 4. Servicio `ServicioUsuarios.cs`
- `RegistrarUsuarioAsync`: Hashea contraseña con BCrypt
- `LoginAsync`: Genera JWT con claims (Id, Email, Nombre, Rol)
- `OlvidarContraseñaAsync`: Genera código, invalida anteriores
- `RestablecerContraseñaAsync`: Valida código, actualiza contraseña

### 5. DTOs Actualizados (`UsuariosDtos.cs`)
- `UsuarioResponseDto.Id` cambiado de `Guid` a `int`
- Añadidos comentarios XML

### 6. Interfaces Corregidas
- `IRepositorioUsuario`: Parámetro `id` de `Guid` a `int`
- `IRepositorioCodigoRecuperacion`: `usuarioId` como `int`

### 7. Entidades Actualizadas
- `Usuario.Id`: `int` autoincremental
- `Usuario.Contraseña`: Renombrado desde `HashContraseña`
- `SolicitudViaje.UsuarioId`: `int`
- `CodigoRecuperacionContraseña.UsuarioId`: `int`

### 8. Archivos Eliminados
- `RecuperacionContraseñaController.cs` (redundante)
- `IRecuperacionContraseñaServices.cs` (integrado en ServicioUsuarios)

---

## 🔧 Dependencias Añadidas

### TravelRequests.Application.csproj
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="10.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.1" />
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.3.1" />
```

---

## 📋 Pendiente (para otras ramas)

1. ⚠️ **Crear nueva migración** para sincronizar cambios de esquema
2. Implementar controlador de Solicitudes de Viaje
3. Añadir FluentValidation a DTOs
4. Middleware de manejo de errores global
5. Configurar Serilog para logging estructurado

---

## 🧪 Cómo Probar

1. Ejecutar: `dotnet run --project TravelRequest`
2. Ir a: `https://localhost:{puerto}/swagger`
3. Registrar usuario: POST `/api/auth/registrar`
4. Login: POST `/api/auth/login` → copiar token
5. Usar token en header: `Authorization: Bearer {token}`
6. Probar `/api/auth/perfil` para verificar

---

## ⚡ Notas de Seguridad

- Contraseñas hasheadas con **BCrypt** (salting automático)
- JWT firmado con **HMAC-SHA256**
- Códigos de recuperación expiran en **5 minutos**
- Token JWT expira en **60 minutos** (configurable)

