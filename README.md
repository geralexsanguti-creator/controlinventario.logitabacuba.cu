# Sistema de Control de Inventario

Sistema de gestión de inventario desarrollado con Blazor WebAssembly y ASP.NET Core API.

## Requisitos Previos

### Software necesario

1. **.NET SDK 10.0** o superior
   - Descargar: https://dotnet.microsoft.com/download/dotnet/10.0
   - Verificar instalación: `dotnet --version`

2. **Visual Studio 2022** (opcional pero recomendado)
   - https://visualstudio.microsoft.com/downloads/

3. **SQLite** (incluido con .NET, no requiere instalación adicional)

4. **Git** (opcional, para clonar el repositorio)
   - https://git-scm.com/download/win

## Pasos de Instalación

### 1. Clonar o descargar el proyecto

Si usas Git:
```bash
git clone <url-del-repositorio>
cd ControlInventario
```

O descarga y descomprime el archivo ZIP del proyecto.

### 2. Restaurar dependencias

```bash
cd ControlInventario
dotnet restore
```

### 3. Crear la base de datos SQLite

```bash
cd ControlInventarioApi
dotnet ef database update
```

Este comando:
- Creará la base de datos `ControlInventario.db`
- Aplicará todas las migraciones
- Insertará los datos iniciales (seed data)

### 4. Ejecutar la aplicación

#### Opción A: Ejecutar ambos proyectos simultáneamente

Desde la raíz del proyecto:

```bash
dotnet run --project ControlInventarioApi/ControlInventarioApi.csproj
dotnet run --project ControlInventarioFrontend/ControlInventarioFrontend.csproj
```

O usar dos terminales separadas:

**Terminal 1 - API:**
```bash
cd ControlInventarioApi
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd ControlInventarioFrontend
dotnet run
```

#### Opción B: Usar Visual Studio

1. Abrir la solución `ControlInventario.sln` en Visual Studio
2. Configurar proyectos de inicio múltiples:
   - Click derecho en la solución > "Set StartUp Projects..."
   - Seleccionar "Multiple startup projects"
   - Establecer "ControlInventarioApi" y "ControlInventarioFrontend" como "Start"
3. Presionar F5 para ejecutar

### 5. Acceder a la aplicación

Una vez ejecutándose:

- **Frontend (Blazor):** http://localhost:xxxxx
- **API (Swagger):** http://localhost:7239/swagger

## Estructura del Proyecto

```
ControlInventario/
├── ControlInventarioApi/           # API REST con ASP.NET Core
│   ├── Controllers/                # Controladores de la API
│   ├── Data/                      # DbContext y configuraciones
│   ├── Models/                     # Modelos de datos
│   ├── Migrations/                 # Migraciones de Entity Framework
│   └── Program.cs                  # Punto de entrada de la API
│
├── ControlInventarioFrontend/      # Aplicación Blazor WebAssembly
│   ├── Pages/                     # Páginas y dashboards
│   ├── Components/                # Componentes reutilizables
│   ├── Services/                  # Servicios de comunicación con API
│   └── Layout/                    # Layouts de la aplicación
│
├── ControlInventarioShared/        # Código compartido
│   └── Models/                    # DTOs y modelos compartidos
│
└── ControlInventario.sln          # Archivo de solución
```

## Funcionalidades Principales

### Módulos del Sistema

1. **Dashboard Principal** - Panel de control central con acceso a todos los módulos
2. **Productos** - Gestión del catálogo de productos con imágenes
3. **Empresas** - Administración de empresas y UEBs
4. **Productores** - Registro de productores agrícolas
5. **Terceros** - Gestión de terceros (proveedores/clientes)
6. **Entradas de Inventario** - Registro de entradas de productos
7. **Salidas de Inventario** - Registro de salidas de productos
8. **Deudas** - Seguimiento de deudas con terceros
9. **Pagos** - Registro de pagos de deudas
10. **Reportes** - Análisis estadístico y gráficos

### Características Técnicas

- **Paginación server-side** en todos los listados
- **Subida de imágenes** comprimidas en base64 (máx. 1 MB)
- **Relaciones geográficas** (Provincias y Municipios de Cuba)
- **Paginación de deudas** por tercero y UEB
- **Dashboard de reportes** con estadísticas

## Solución de Problemas

### Error: "address already in use"

Si el puerto 7239 o 5xxx está en uso:

```powershell
# Encontrar el proceso usando el puerto
netstat -ano | findstr :7239

# Matar el proceso (reemplazar PID con el número encontrado)
taskkill /PID <PID> /F
```

### Error: "Cannot provide a value for property"

Este error indica que falta registrar un servicio. Verificar que todos los servicios estén registrados en `Program.cs` del frontend.

### La base de datos no se crea

```bash
cd ControlInventarioApi
dotnet ef database drop --force
dotnet ef database update
```

### Error de compilación con migraciones pendientes

```bash
cd ControlInventarioApi
dotnet build
dotnet ef migrations add <NombreMigracion>
dotnet ef database update
```

## Configuración Adicional

### Cambiar puertos

**API** (`ControlInventarioApi/Properties/launchSettings.json`):
```json
"applicationUrl": "http://localhost:7239"
```

**Frontend** (`ControlInventarioFrontend/Program.cs`):
```csharp
BaseAddress = new Uri("http://localhost:7239/")
```

### Base de datos SQLite

La base de datos se crea automáticamente en la carpeta del proyecto API.

Para usar SQL Server en lugar de SQLite, modificar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ControlInventario;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## Comandos Útiles

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar pruebas (si existen)
dotnet test

# Crear nueva migración
dotnet ef migrations add <NombreMigracion>

# Aplicar migraciones
dotnet ef database update

# Eliminar base de datos
dotnet ef database drop

# Ver información del DbContext
dotnet ef dbcontext info
```

## Licencia

Este proyecto es de uso interno.

---

## Deploy en Producción (Windows 11)

### Opción 1: Publicación como Aplicación Autocontenida

Esta opción genera archivos ejecutables que no requieren .NET instalado en el servidor.

#### 1. Publicar la API

```bash
cd ControlInventarioApi
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../publish/api
```

#### 2. Publicar el Frontend

```bash
cd ControlInventarioFrontend
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../publish/frontend
```

#### 3. Estructura de archivos generada

```
publish/
├── api/
│   ├── ControlInventarioApi.exe    # Ejecutable de la API
│   ├── ControlInventarioApi.dll
│   ├── ControlInventario.db         # Base de datos (se creará al ejecutar)
│   └── ...
└── frontend/
    ├── wwwroot/                    # Archivos estáticos
    └── ...
```

#### 4. Configurar el Frontend para Producción

Editar `ControlInventarioFrontend/wwwroot/appsettings.json` (crear si no existe):

```json
{
  "ApiUrl": "http://192.168.1.100:5000/"
}
```

O modificar `Program.cs` con la URL de producción:

```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://192.168.1.100:5000/") });
```

#### 5. Iniciar la API

```powershell
cd publish/api
.\ControlInventarioApi.exe --urls "http://0.0.0.0:5000"
```

#### 6. Configurar el Frontend para IIS o Nginx

Copiar el contenido de `publish/frontend/wwwroot` a la carpeta del sitio web.

### Opción 2: Usando IIS (Internet Information Services)

#### 1. Instalar IIS en Windows 11

1. Abrir "Panel de Control" > "Programas" > "Activar o desactivar características de Windows"
2. Marcar **Internet Information Services**
3. Marcar también:
   - IIS Management Console
   - World Wide Web Services > Application Development Features > ASP.NET 4.8 (si aparece)
4. Aceptar y esperar a que se instale

#### 2. Instalar .NET Hosting Bundle

Descargar e instalar: https://dotnet.microsoft.com/download/dotnet/10.0

Buscar "Windows Hosting Bundle" para IIS.

#### 3. Configurar permisos

```powershell
# Dar permisos a la carpeta de la aplicación
icacls "C:\inetpub\wwwroot\ControlInventario" /grant "IIS_IUSRS:(OI)(CI)RX"
icacls "C:\inetpub\wwwroot\ControlInventario" /grant "IIS_IUSRS:(OI)(CI)M"
```

#### 4. Publicar para IIS

```bash
cd ControlInventarioApi
dotnet publish -c Release -o C:\inetpub\wwwroot\ControlInventarioApi
```

#### 5. Crear aplicación en IIS

1. Abrir ** IIS Manager** (inetmgr)
2. Click derecho en "Sites" > "Add Website"
3. Configurar:
   - Site name: `ControlInventarioApi`
   - Physical path: `C:\inetpub\wwwroot\ControlInventarioApi`
   - Binding: `http`, Port `5000`, IP `*`
4. Application Pool: Crear nuevo con ".NET CLR Version" = "No Managed Code"

#### 6. Configurar CORS (si es necesario)

En `appsettings.Production.json` o `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://tudominio.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

### Opción 3: Usando Windows Service (sin IIS)

Crear un servicio de Windows que ejecute la API automáticamente.

#### 1. Crear el servicio con NSSM

1. Descargar NSSM: https://nssm.cc/download
2. Extraer a `C:\nssm`

#### 2. Instalar el servicio

```powershell
C:\nssm\win64\nssm.exe install ControlInventarioApi
```

Configurar en la interfaz de NSSM:
- **Path**: `C:\publish\api\ControlInventarioApi.exe`
- **Arguments**: `--urls "http://0.0.0.0:5000"`
- **Startup directory**: `C:\publish\api`

#### 3. Iniciar el servicio

```powershell
net start ControlInventarioApi
```

### Opción 4: Docker (alternativa recomendada)

#### 1. Crear Dockerfile para la API

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY publish/api/ .
EXPOSE 5000
ENTRYPOINT ["dotnet", "ControlInventarioApi.dll"]
```

#### 2. Crear docker-compose.yml

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:5000"
    volumes:
      - ./data:/app/data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

#### 3. Ejecutar con Docker

```bash
docker-compose up -d
```

### Configuración de Producción Recomendada

#### 1. appsettings.Production.json (API)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 2. Variables de entorno importantes

```powershell
# Producción
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"

# Ruta de la base de datos
$env:ConnectionStrings__DefaultConnection = "Data Source=/app/data/ControlInventario.db"
```

### Configuración del Firewall (Windows)

```powershell
# Abrir puertos en el firewall de Windows
New-NetFirewallRule -DisplayName "ControlInventario API" -Direction Inbound -Protocol TCP -LocalPort 5000 -Action Allow
New-NetFirewallRule -DisplayName "ControlInventario Frontend" -Direction Inbound -Protocol TCP -LocalPort 5001 -Action Allow
```

### Verificar el Deploy

1. Probar la API:
   ```
   http://localhost:5000/swagger
   ```

2. Probar el Frontend:
   ```
   http://localhost:5001/
   ```

3. Verificar logs en caso de errores:
   ```
   %LOCALAPPDATA%\ControlInventario\logs\
   ```

### Resumen Rápido de Deploy

```powershell
# 1. Publicar ambos proyectos
cd ControlInventarioApi
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../publish/api

cd ../ControlInventarioFrontend  
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../publish/frontend

# 2. Copiar base de datos
Copy-Item ControlInventarioApi/ControlInventario.db ../publish/api/

# 3. Iniciar
cd ../publish/api
.\ControlInventarioApi.exe --urls "http://0.0.0.0:5000"
```
