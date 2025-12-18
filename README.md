# 🏨 Hotel Viva Bueno - Sistema de Gestión de Empleados

Sistema completo de gestión de empleados desarrollado con **ASP.NET Core 8**, **PostgreSQL** (Railway), **Vue.js**, **JWT**, **SMTP** y **Docker**.

---

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Requisitos](#-requisitos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Docker Compose](#-docker-compose)
- [Testing](#-testing)
- [Credenciales](#-credenciales)
- [API Endpoints](#-api-endpoints)
- [Estructura del Proyecto](#-estructura-del-proyecto)

---

## ✨ Características

### Aplicación Web (Admin RRHH)
- ✅ **Autenticación**: Cookie-based con ASP.NET Identity
- ✅ **Dashboard**: Estadísticas en tiempo real (Total, Activos, En Vacaciones)
- ✅ **CRUD Empleados**: Gestión completa con validaciones
- ✅ **Importación Excel**: Carga masiva con EPPlus
- ✅ **PDF Hoja de Vida**: Generación dinámica con QuestPDF
- ✅ **Gestión Departamentos**: Asignación y relaciones FK

### API REST
- ✅ **Swagger UI**: Documentación interactiva
- ✅ **JWT Authentication**: Tokens seguros
- ✅ **SMTP**: Envío de emails con Gmail
- ✅ **Endpoints Públicos**: Registro, Login, Departamentos
- ✅ **Endpoints Protegidos**: Datos empleado, Descarga CV

### Base de Datos
- ✅ **PostgreSQL**: Railway cloud database
- ✅ **Entity Framework Core**: ORM con migraciones
- ✅ **Seed Data**: 3 departamentos iniciales
- ✅ **Índices**: Únicos en Email y DocumentNumber

---

## 🏗️ Arquitectura

```
┌─────────────┐      ┌─────────────┐      ┌──────────────┐
│   Browser   │─────▶│  Web (MVC)  │─────▶│  PostgreSQL  │
│  (Vue.js)   │      │  Port 5000  │      │   Railway    │
└─────────────┘      └─────────────┘      └──────────────┘
                            │
                            ▼
                     ┌─────────────┐
                     │  API REST   │
                     │  Port 5001  │
                     │   Swagger   │
                     └─────────────┘
```

- **Web**: ASP.NET Core MVC + Vue.js (CDN) + Bootstrap
- **API**: ASP.NET Core Web API + JWT + Swagger
- **Data**: Entity Framework Core + PostgreSQL

---

## 🛠️ Tecnologías

| Componente | Tecnología | Versión |
|------------|------------|---------|
| Framework | ASP.NET Core | 8.0 |
| Lenguaje | C# | 12 |
| Base de Datos | PostgreSQL | 16 (Railway) |
| ORM | Entity Framework Core | 8.0 |
| Frontend | Vue.js | 3.4 (CDN) |
| Estilos | Bootstrap | 5.3 |
| Auth API | JWT Bearer | 8.0 |
| Auth Web | Cookie Authentication | 8.0 |
| Excel | EPPlus | 8.4 |
| PDF | QuestPDF | 2024.12 |
| Email | MailKit | 4.9 |
| Testing | xUnit + Moq | 2.4 |
| Containerización | Docker + Compose | Latest |

---

## 📦 Requisitos

### Desarrollo Local
- ✅ [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- ✅ [PostgreSQL Client](https://www.postgresql.org/download/) (opcional, para verificar)
- ✅ Cuenta [Railway](https://railway.app/) (PostgreSQL en la nube)
- ✅ Cuenta Gmail con [App Password](https://support.google.com/accounts/answer/185833)

### Producción (Docker)
- ✅ [Docker Engine 20.10+](https://docs.docker.com/engine/install/)
- ✅ [Docker Compose 2.0+](https://docs.docker.com/compose/install/)

---

## 🚀 Instalación

### 1. Clonar Repositorio
```bash
git clone <URL_DEL_REPOSITORIO>
cd HotelVidaBuena
```

### 2. Restaurar Dependencias
```bash
dotnet restore
```

### 3. Configurar Database (Railway)

**Railway ya tiene la BD configurada con:**
- Host: `maglev.proxy.rlwy.net`
- Port: `28849`
- Database: `railway`
- User: `postgres`
- Password: `XRjADCHlaTOiZwEObCbzgdHxRrrrFPbO`

**Las migraciones ya están aplicadas** con las tablas:
- `Departments` (3 registros seed)
- `Employees` (con usuario admin)

---

## ⚙️ Configuración

### Archivo: `appsettings.json` (Web y API)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=maglev.proxy.rlwy.net;Port=28849;Database=railway;Username=postgres;Password=XRjADCHlaTOiZwEObCbzgdHxRrrrFPbO;SSL Mode=Prefer"
  },
  "Jwt": {
    "Secret": "SuperSecretKeyForDevelopmentOnly12345!_ChangeMeInProd"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "User": "tu.email@gmail.com",
    "Pass": "tu_app_password_16_chars"
  }
}
```

> ⚠️ **Importante**: Reemplaza `SMTP User` y `Pass` con tus credenciales reales de Gmail.

---

## 🎯 Ejecución

### Opción 1: Desarrollo Local

**Terminal 1 - Web Application:**
```bash
dotnet run --project HotelVivaBueno.Web/HotelVivaBueno.Web.csproj
```
📍 Abre: http://localhost:5000

**Terminal 2 - API REST:**
```bash
dotnet run --project HotelVivaBueno.Api/HotelVivaBueno.Api.csproj
```
📍 Swagger: http://localhost:5001/swagger

### Opción 2: Build de Producción
```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet HotelVivaBueno.Web.dll
```

---

## 🐳 Docker Compose

### 1. Configurar Variables de Entorno
```bash
cp .env.example .env
# Editar .env con tus credenciales
```

### 2. Build y Ejecutar
```bash
docker-compose up --build
```

### 3. Acceder
- **Web**: http://localhost:5000
- **API**: http://localhost:5001

### 4. Detener
```bash
docker-compose down
```

---

## 🧪 Testing

### Ejecutar Todas las Pruebas
```bash
dotnet test
```

### Pruebas Específicas
```bash
# Solo unitarias
dotnet test --filter "FullyQualifiedName~Unit"

# Solo integración
dotnet test --filter "FullyQualifiedName~Integration"
```

### Cobertura Actual
- ✅ 2 Pruebas Unitarias (AuthService)
- ✅ 2 Pruebas de Integración (API Endpoints)
- 📊 Ratio de Éxito: 80% (4/5)

---

## 🔐 Credenciales

### Usuario Admin (Web)
- **Documento**: `1234567890`
- **Password**: `Admin123!`

### Departamentos (Pre-cargados)
1. Recursos Humanos
2. Tecnología
3. Operaciones

### SMTP (Configurar Propio)
1. Ve a [Configuración de Google](https://myaccount.google.com/security)
2. Habilita "Verificación en 2 pasos"
3. Genera "Contraseña de aplicación"
4. Usa esa contraseña en `appsettings.json`

---

## 📡 API Endpoints

### Públicos (Sin Autenticación)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/departments` | Lista todos los departamentos |
| POST | `/auth/register` | Registro de empleado + email |
| POST | `/auth/login` | Login, retorna JWT token |

### Protegidos (Requiere JWT)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/employees/me` | Datos del empleado autenticado |
| GET | `/employees/me/cv` | Descarga PDF de hoja de vida |

### Ejemplo: Login
```bash
curl -X POST http://localhost:5001/auth/login \
  -H "Content-Type: application/json" \
  -d '{"documentNumber":"1234567890","password":"Admin123!"}'
```

Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Ejemplo: Uso del Token
```bash
curl -X GET http://localhost:5001/employees/me \
  -H "Authorization: Bearer {TU_TOKEN_AQUI}"
```

---

## 📁 Estructura del Proyecto

```
HotelVivaBueno/
├── HotelVivaBueno.Web/          # ASP.NET Core MVC
│   ├── Controllers/              # HomeController, EmployeesController, AccountController
│   ├── Views/                    # Razor Views (Dashboard, CRUD)
│   ├── Models/                   # ViewModels
│   └── Dockerfile               # Docker image para Web
│
├── HotelVivaBueno.Api/          # ASP.NET Core Web API
│   ├── Controllers/              # AuthController, DepartmentsController, EmployeesController
│   ├── Services/                 # AuthService, EmailService, PdfService
│   ├── DTOs/                     # Data Transfer Objects
│   └── Dockerfile               # Docker image para API
│
├── HotelVivaBueno.Data/         # Entity Framework Core
│   ├── Entities/                 # Employee, Department, BaseEntity
│   ├── Enums/                    # EmployeeStatus, EducationLevel
│   ├── Data/                     # ApplicationDbContext, Factory
│   └── Migrations/               # EF Core Migrations
│
├── HotelVivaBueno.Tests/        # xUnit Tests
│   ├── Unit/                     # AuthServiceTests
│   └── Integration/              # ApiIntegrationTests
│
├── docker-compose.yml           # Multi-container orchestration
├── .env.example                 # Environment variables template
├── .dockerignore                # Docker build exclusions
└── README.md                    # This file
```

---

## 📝 Documentación Adicional

- `MIGRACION_MANUAL.md` - Instrucciones para aplicar migraciones manualmente
- `IPv6_SOLUTION.md` - Soluciones para problema de conectividad IPv6
- `TEST_RESULTS.md` - Resultados detallados de las pruebas
- `walkthrough.md` - Demostración completa del sistema

---

## 🎓 Licencias

- **EPPlus**: NonCommercial (para uso educativo/pruebas técnicas)
- **QuestPDF**: Community License
- **Proyecto**: Prueba Técnica - Sistema Hotel Viva Bueno

---

## 👨‍💻 Autor

Desarrollado como prueba técnica para demostración de habilidades en:
- ASP.NET Core MVC & Web API
- Entity Framework Core
- PostgreSQL
- Vue.js
- Docker & Docker Compose
- Testing (xUnit)
- JWT Authentication
- SMTP Integration

---

## 🐛 Troubleshooting

### Error: IPv6 Network Unreachable
Ver archivo `IPv6_SOLUTION.md` para soluciones detalladas.

### Error: EPPlus License
Ya está solucionado removiendo la configuración obsoleta del `Program.cs`.

### Error: Database Connection
Verificar que Railway PostgreSQL esté accesible y las credenciales sean correctas.

---

**Última Actualización**: Diciembre 2025  
**Versión**: 1.0.0
