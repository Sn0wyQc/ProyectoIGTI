# SkillSwap

> Aplicación móvil multiplataforma para el intercambio de habilidades entre usuarios, desarrollada con **.NET MAUI** y arquitectura **MVVM**.

---

## Descripción

**SkillSwap** es una app que conecta personas que desean compartir y aprender habilidades. Los usuarios pueden publicar anuncios con sus habilidades, explorar los de otros, chatear directamente y gestionar su perfil personal, todo desde una interfaz intuitiva con navegación lateral.

---

## Funcionalidades

- **Autenticación segura** — Registro e inicio de sesión con hash SHA-256
- **Perfil editable** — Nombre, correo y descripción de habilidades
- **Feed de anuncios** — CRUD completo de publicaciones
- **Filtro por categorías** — Explora habilidades por tipo
- **Chat entre usuarios** — Mensajería directa con historial persistente
- **Notificaciones internas** — Sistema de mensajería en tiempo real con `WeakReferenceMessenger`
- **Navegación Shell** — Flyout lateral para moverse entre secciones
- **Arquitectura en capas + MVVM Toolkit**

---

## Tecnologías utilizadas

| Tecnología | Descripción |
|---|---|
| .NET MAUI | Framework multiplataforma (Android, iOS, Windows) |
| C# | Lenguaje principal |
| XAML | Diseño de interfaces |
| SQLite | Base de datos local |
| CommunityToolkit.Mvvm | Implementación del patrón MVVM |
| CommunityToolkit.Maui | Controles y helpers adicionales |

---

## Paquetes NuGet

| Paquete | Versión |
|---|---|
| `CommunityToolkit.Mvvm` | 8.2.2 |
| `CommunityToolkit.Maui` | 7.0.1 |
| `sqlite-net-pcl` | 1.9.172 |
| `SQLitePCLRaw.bundle_green` | 2.1.8 |

> Los paquetes se restauran automáticamente al hacer **Build** del proyecto.

---

## Estructura del proyecto

```
SkillSwap/
├── Models/           → Clases de datos (User, Post, Message, Skill)
├── ViewModels/       → Lógica de presentación con MVVM
├── Views/            → Páginas XAML de la interfaz
├── Services/         → Lógica de negocio y acceso a datos
├── Resources/        → Imágenes, fuentes y recursos estáticos
├── Platforms/        → Código específico por plataforma
├── AppShell.xaml     → Navegación principal (Flyout)
├── MauiProgram.cs    → Configuración e inyección de dependencias
└── SkillSwap.csproj  → Configuración del proyecto
```

---

## Cómo ejecutar el proyecto

### Requisitos previos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) con la carga de trabajo **.NET MAUI**
- SDK de Android instalado (para emulador o dispositivo físico)

### Pasos

1. **Clonar** el repositorio:
   ```bash
   git clone https://github.com/Sn0wyQc/ProyectoIGTI.git
   ```

2. **Abrir** `SkillSwap.sln` en Visual Studio 2022

3. Click derecho en el proyecto → **Restore NuGet Packages**

4. Seleccionar un **emulador Android** o conectar un dispositivo físico

5. Presionar **F5** o el botón ▶️ **Run**

---

## Cuentas de prueba

La base de datos se genera automáticamente al iniciar la app. Para comenzar, regístrate desde la pantalla de inicio con:

- **Nombre** de usuario
- **Correo electrónico**
- **Contraseña** (mínimo 6 caracteres)
- **Descripción** de tus habilidades

---

## Notas técnicas

- La base de datos SQLite se crea automáticamente en `FileSystem.AppDataDirectory`
- El `WeakReferenceMessenger` gestiona notificaciones de nuevos mensajes en tiempo real dentro de la misma sesión

---

## Contribuidores

| Usuario | GitHub |
|---|---|
| Mauricio Castilla | [@Sn0wyQc](https://github.com/Sn0wyQc) |
| Yovany Nahuat | [@BILGAX07](https://github.com/Cristian5546) |
| Cristian Canul | [@Cristian5546](https://github.com/BILGAX07) |
| Henri Cauich | [@hcauich25130-bit](https://github.com/hcauich25130-bit) |
| Carlos Cardeña | [@ccardena26405-hash](https://github.com/ccardena26405-hash) |

---

