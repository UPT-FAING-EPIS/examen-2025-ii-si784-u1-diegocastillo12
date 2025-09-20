# Red Social Universitaria

Una plataforma web completa para la interacción entre estudiantes, docentes y personal universitario.

## Estructura del Proyecto

```
examen_calidad/
├── backend/                    # API .NET Core
│   ├── UniversitySocialNetwork.API/
│   ├── UniversitySocialNetwork.Core/
│   ├── UniversitySocialNetwork.Infrastructure/
│   └── UniversitySocialNetwork.Tests/
├── frontend/                   # Aplicación React
│   ├── src/
│   ├── public/
│   └── package.json
├── infrastructure/             # Terraform IaC
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── .github/                   # GitHub Actions
│   └── workflows/
└── docs/                      # Documentación
```

## Funcionalidades

- ✅ Gestión de perfiles de usuario
- ✅ Publicaciones y comentarios
- ✅ Grupos y comunidades
- ✅ Mensajería privada
- ✅ Búsqueda avanzada
- ✅ Panel de administración

## Tecnologías

- **Backend**: .NET Core 8, Entity Framework Core, SQL Server
- **Frontend**: React 18, TypeScript, Material-UI
- **Base de Datos**: SQL Server
- **Infraestructura**: Azure, Terraform
- **CI/CD**: GitHub Actions
- **Análisis de Código**: SonarCloud, Semgrep, Snyk

## Instalación y Configuración

### Backend
```bash
cd backend/UniversitySocialNetwork.API
dotnet restore
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm start
```

## Análisis de Calidad

El proyecto incluye análisis automático de código con:
- SonarCloud para calidad y seguridad
- Semgrep para detección de vulnerabilidades
- Snyk para análisis de dependencias

## Despliegue

El despliegue se realiza automáticamente mediante GitHub Actions a Azure.