# TechManager

Sistema de gestión centralizada de conocimientos técnico-dispositivo para laboratorios de reparación electrónica.

## 📋 Descripción

TechManager es una aplicación de escritorio desarrollada como proyecto final de DAM (Desarrollo de Aplicaciones Multiplataforma) que soluciona la fragmentación de información en laboratorios técnicos, centralizando:

- Gestión de técnicos y dispositivos
- Asignación de conocimientos técnico-dispositivo
- Registro y seguimiento de actualizaciones
- Verificación de actualizaciones

## 🚀 Características principales

- **Gestión CRUD completa** de técnicos y dispositivos
- **Asignación de conocimientos** técnico-dispositivo
- **Seguimiento de actualizaciones** de dispositivos
- **Verificación de actualizaciones** para control supervisor
- **Sincronización en tiempo real** entre ventanas
- **Arquitectura limpia** con separación de capas
- **Base de datos SQLite** embebida

## 🛠️ Tecnologías

- C# / .NET 8
- WPF (Windows Presentation Foundation)
- SQLite
- xUnit (testing)
- Patrones: MVVM, Repository, Dependency Injection, Observer

## 📦 Instalación

### Requisitos del sistema
- Windows 10 o superior (64 bits)
- 2 GB RAM mínimo
- 200 MB espacio en disco
- No requiere instalación de .NET (incluido en el paquete)

### Descarga
Descarga la última versión desde [Releases](https://github.com/cesarforall/TechManager/releases/latest)

### Pasos
1. Descomprimir `TechManager-v1.0.0.zip` en una carpeta
2. Ejecutar `TechManager.exe`
3. (Opcional) Para datos de prueba:
   - Copiar `sample-data/techmanager-sample.db`
   - Renombrarlo a `techmanager.db`
   - Colocarlo junto al ejecutable antes del primer inicio

## 🏗️ Estructura del proyecto
```
TechManager/
├── TechManager.UI/          # Capa de presentación (WPF + MVVM)
├── TechManager.Core/        # Lógica de negocio y entidades
├── TechManager.Data/        # Acceso a datos y repositorios
└── TechManager.Tests/       # Pruebas unitarias
```

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver archivo [LICENSE](LICENSE) para más detalles.
