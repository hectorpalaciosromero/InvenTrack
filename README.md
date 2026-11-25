# InvenTrack

Sistema de Gestión de Inventario con Simulación de Captura Automática (RFID / QR / Código de Barras).

## Descripción

Aplicación de escritorio desarrollada en **WPF (.NET 8)** con arquitectura **MVVM**, diseñada para la gestión integral de inventarios en entornos de comercio mayorista y minorista. Incluye un módulo de simulación de dispositivos de captura automática (RFID, QR, código de barras) para el registro de entradas, salidas y transferencias de mercadería.

## Tecnologías

| Componente | Tecnología |
|---|---|
| Lenguaje | C# 12 |
| Framework | .NET 8 / WPF |
| Patrón | MVVM |
| Base de Datos | SQL Server (LocalDB) |
| ORM | Dapper |
| Hashing | BCrypt.Net |

## Estructura del Proyecto

```
InvenTrack/
├── database/              Scripts SQL (tablas, SP, vistas, triggers)
├── docs/                  Documentación técnica
└── src/InvenTrack/
    └── InvenTrack/
        ├── Core/          Clases base MVVM
        ├── Models/        Entidades del dominio
        ├── Data/          Repositorios y acceso a datos
        ├── Services/      Lógica de negocio
        ├── ViewModels/    ViewModels (MVVM)
        ├── Views/         Interfaces de usuario (XAML)
        ├── Converters/    Value Converters
        └── Themes/        Estilos y recursos visuales
```

## Módulos

- **Autenticación**: Login con roles (Administrador / Operario)
- **Dashboard**: Indicadores clave de inventario en tiempo real
- **Gestión de Productos**: CRUD completo con categorías y códigos de identificación
- **Simulador de Captura**: Simulación de lectura RFID, QR y código de barras
- **Movimientos de Almacén**: Registro de entradas, salidas y transferencias
- **Reportes**: Stock actual, movimientos, alertas de stock mínimo

## Requisitos

- .NET 8 SDK
- SQL Server LocalDB (incluido con Visual Studio)
- Visual Studio 2022+

## Instalación

1. Clonar el repositorio.
2. Ejecutar los scripts SQL en orden (`database/01_CreateDatabase.sql` a `06_Triggers.sql`).
3. Configurar la cadena de conexión en `appsettings.json`.
4. Compilar y ejecutar desde Visual Studio o con `dotnet run`.

## Usuarios por defecto

| Usuario | Contraseña | Rol |
|---|---|---|
| admin | Admin123! | Administrador |
| operario | Oper123! | Operario |
