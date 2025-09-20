# Red Social Universitaria

[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/JdReedF3)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=20615530)

Una plataforma web completa para la interacción entre estudiantes, docentes y personal universitario.

## 📋 Avance del Proyecto - Examen de Calidad de Software

### ✅ Funcionalidades Completadas

#### 🏗️ Arquitectura Base
- ✅ Backend .NET Core 8 con Clean Architecture
- ✅ Frontend React 18 + TypeScript + Material-UI
- ✅ Base de datos SQL Server con Entity Framework Core
- ✅ Autenticación JWT implementada

#### 📊 Funcionalidades de Red Social
- ✅ Gestión de perfiles de usuario (Student, Teacher, Staff, Admin)
- ✅ Sistema de publicaciones con multimedia
- ✅ Comentarios y reacciones (Like, Love, Laugh)
- ✅ Grupos académicos y sociales con roles
- ✅ Mensajería privada entre usuarios
- ✅ Búsqueda avanzada

#### 📚 Documentación Automatizada (COMPLETADO)
- ✅ **Generación automática de diagrama de clases** con PlantUML
- ✅ **Documentación automática del código** (API + Frontend)
- ✅ Workflow de GitHub Actions para automatización
- ✅ README completo con arquitectura y guías
- ✅ Configuración TypeDoc para frontend
- ✅ Documentación XML para .NET API

### 🚧 Próximas Tareas Pendientes

#### ☁️ Infraestructura en Nube (Prioridad Media)
- ⏳ Terraform para Azure/AWS
- ⏳ Configuración de recursos cloud
- ⏳ Variables de entorno para producción

#### 🚀 CI/CD Avanzado (Prioridad Media)
- ⏳ Pipeline completo de despliegue
- ⏳ Automatización de releases
- ⏳ Rollback automático

#### 🔍 Análisis de Calidad y Seguridad (Prioridad Media)
- ⏳ SonarCloud para calidad de código
- ⏳ Semgrep para vulnerabilidades
- ⏳ Snyk para análisis de dependencias

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
├── .github/workflows/          # GitHub Actions
│   └── documentation.yml      # ✅ Automatización docs
├── docs/                      # ✅ Documentación completa
│   └── README.md
├── infrastructure/             # ⏳ Terraform IaC
└── typedoc.json               # ✅ Config TypeDoc
```

## Tecnologías

- **Backend**: .NET Core 8, Entity Framework Core, SQL Server
- **Frontend**: React 18, TypeScript, Material-UI
- **Base de Datos**: SQL Server
- **Infraestructura**: Azure, Terraform
- **CI/CD**: GitHub Actions
- **Análisis de Código**: SonarCloud, Semgrep, Snyk
- **Documentación**: PlantUML, TypeDoc, Swagger/OpenAPI

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

## 📈 Estado del Proyecto

- **Documentación Automatizada**: ✅ **COMPLETADO**
- **Funcionalidades Core**: ✅ **COMPLETADO** 
- **Infraestructura Cloud**: ⏳ **PENDIENTE**
- **CI/CD Avanzado**: ⏳ **PENDIENTE**
- **Análisis de Calidad**: ⏳ **PENDIENTE**

## 🎯 Próximo Objetivo

Implementar **Infraestructura como Código** con Terraform para completar el stack de DevOps moderno.