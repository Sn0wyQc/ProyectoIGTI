<<<<<<< HEAD

##  Paquetes NuGet necesarios

Estos se instalan automáticamente al hacer Build, pero puedes verificarlos:

| Paquete | Versión |
|---|---|
| CommunityToolkit.Mvvm | 8.2.2 |
| CommunityToolkit.Maui | 7.0.1 |
| sqlite-net-pcl | 1.9.172 |
| SQLitePCLRaw.bundle_green | 2.1.8 |


##  Pasos para ejecutar

1. **Abrir** el proyecto en Visual Studio 2022
2. Click derecho en el proyecto -> **Restore NuGet Packages**
3. Seleccionar emulador Android o dispositivo físico
4. Presionar **F5** o el botón  Run



## Estructura del proyecto

```
SkillSwap/
├── Models/           → Clases de datos (User, Post, Message, Skill)
├── ViewModels/       → Lógica de presentación con MVVM
├── Views/            → Páginas XAML
├── Services/         → Lógica de negocio y acceso a datos
├── AppShell.xaml     → Navegación principal (Flyout)
├── MauiProgram.cs    → Inyección de dependencias
└── SkillSwap.csproj  → Configuración del proyecto
```



##  Cuentas de prueba

Al no tener datos, regístrate desde la app con:
- Nombre, correo, contraseña (mín. 6 caracteres)
- Describe tus habilidades



##  Notas importantes

- La base de datos SQLite se crea automáticamente en `AppDataDirectory`
- El WeakReferenceMessenger notifica mensajes nuevos en tiempo real (dentro de la misma sesión)
- Las imágenes `pencil.png` y `trash.png` deben estar en `Resources/Images/`



##  Funcionalidades implementadas

-  Registro e inicio de sesión con hash SHA-256
-  Perfil editable
-  Feed con CRUD completo de anuncios
-  Filtro por categorías
-  Chat entre usuarios con historial
-  Notificaciones internas con WeakReferenceMessenger
-  Navegación Shell con Flyout lateral
-  Arquitectura en capas + MVVM Toolkit
=======
# ProyectoIGTI
>>>>>>> 77cd76c8362e4846271724bbea8d00ae49bfc8da
