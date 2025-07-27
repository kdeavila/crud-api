# CRUD API con .NET 9 y SQL Server

## ✨ Descripción del Proyecto

Este proyecto demuestra una API Web robusta y estructurada con **ASP.NET Core (.NET 9)** y **Entity Framework Core**, enfocada en la gestión de empleados, perfiles y usuarios. Implementa **Clean Architecture**, **separación de responsabilidades** (Controladores, Servicios, Entidades, DTOs) y **validación de datos exhaustiva**, proporcionando una base escalable y mantenible.

## 🚀 Características Clave

* **Arquitectura por Capas:** Separación clara entre la presentación (Controladores), lógica de negocio (Servicios) y acceso a datos (EF Core).
* **Validación de Entrada Robusta:** Con `Data Annotations` en DTOs y validaciones de reglas de negocio en Servicios (ej. unicidad).
* **Manejo de Errores y Logging Robusto:** Gestión centralizada y contextualizada de excepciones (`DbUpdateException`, `SqlException`), retornando `ServiceResultStatus` precisos (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `409 Conflict`).
* **Endpoints RESTful:** Adhiere a los principios RESTful para las operaciones CRUD (Crear, Leer, Actualizar, Borrar) de recursos.
* **Eficiencia en Acceso a Datos:** Entity Framework Core para interacciones optimizadas.
* **Integración con SQL Server:** Utiliza SQL Server como motor de base de datos relacional.
* **Paginación y Filtrado Avanzado:** Implementación de un sistema flexible de paginación y filtrado, utilizando DTOs genéricos y reutilizables.

## 🔑 Seguridad y Autenticación

* **Autenticación Basada en Tokens JWT:** Flujo de autenticación seguro con JWT, manejando roles (`Admin`, `Manager`, `Viewer`).
* **Gestión Segura de Contraseñas:** Contraseñas almacenadas con hashing **BCrypt** (salting automático).
* **Autorización Basada en Roles (RBAC):** Control de acceso granular a endpoints mediante roles, validados con `[Authorize(Roles = "")]`.
* **Validación de Roles Robusta:** Validación personalizada para asegurar que los valores de rol proporcionados correspondan a roles válidos.

## 🛠️ Tecnologías Utilizadas

* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **BCrypt.Net-Core** (para hashing de contraseñas)
* **DBeaver** (o SQL Server Management Studio para gestión de BD)

## 📂 Estructura del Proyecto

El repositorio está organizado en dos proyectos principales:

*   `crud-api`: Contiene el proyecto principal de la API web de ASP.NET Core. Aquí se encuentran los controladores, servicios, entidades y toda la lógica de la aplicación.
*   `crud-api.UnitTests`: Contiene las pruebas unitarias para los servicios de la aplicación, asegurando la calidad y el correcto funcionamiento de la lógica de negocio.

## ▶️ Cómo Empezar

### ✔️ Prerequisitos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* SQL Server (puedes usar SQL Server Express, LocalDB o una instancia completa)
* Una herramienta para la gestión de bases de datos como DBeaver o SQL Server Management Studio (opcional, para inspección).
* [Insomnia](https://insomnia.rest/download) (o Postman, aunque la colección proporcionada es para Insomnia).

### ⚙️ Configuración del Proyecto

1.  **Clona el repositorio:**
    ```bash
    git clone [https://github.com/kdeavila/crud-api.git](https://github.com/kdeavila/crud-api.git)
    cd crud-api
    ```
2.  **Configura la Conexión a la Base de Datos:**
    * Abre el archivo `appsettings.json` en la raíz del proyecto.
    * Actualiza la cadena de conexión `DefaultConnection` para que apunte a tu instancia de SQL Server.
        *Ejemplo (ajusta según tu configuración):*
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=DESKTOP-M6D4FCH\\MSSQLSERVER01;Database=DbEmployee;Integrated Security=True;TrustServerCertificate=True"
        }
        ```
        *(Asegúrate de que `TrustServerCertificate=True` esté presente si usas certificados auto-firmados o no validados en un entorno local).*
3.  **Aplica las Migraciones de la Base de Datos:**
    * Abre una terminal en la raíz del proyecto (donde se encuentra `crud-api.csproj`).
    * Ejecuta los comandos de Entity Framework Core para crear y actualizar el esquema de la base de datos:
        ```bash
        dotnet ef database update
        ```
    * *(Si es la primera vez que creas migraciones, primero ejecuta `dotnet ef migrations add InitialCreate` y luego `dotnet ef database update`)*.

### ▶️ Ejecutar la API

1.  Abre una terminal en la raíz del proyecto (`crud-api.csproj`).
2.  Ejecuta el siguiente comando:
    ```bash
    dotnet run
    ```
3.  La API se iniciará y típicamente escuchará en `https://localhost:7000` (o un puerto diferente, revisa la salida de la consola para la URL exacta).

## 🌐 Endpoints de la API con Insomnia

Para facilitar la interacción con la API, se proporciona una colección de Insomnia preconfigurada con todas las peticiones necesarias.

### 📥 Importar la Colección de Insomnia

1.  Abre Insomnia.
2.  Ve a `File > Import > From File` (o `Archivo > Importar > Desde Archivo`).
3.  Selecciona el archivo `Insomnia.yaml` que se encuentra en la raíz de este repositorio.
4.  Esto importará la colección "CRUD API" con sus carpetas (`Auth`, `Employees`, `Profiles`, `Users`) y todas las solicitudes preconfiguradas.

### 🔑 Autenticación y Gestión de Tokens

* Tu API requiere autenticación JWT para la mayoría de los endpoints. La colección de Insomnia automatiza la gestión de tokens:
    1.  Realiza un `POST` a `/api/auth/login` con credenciales de usuario.
    2.  El **Bearer Token** se extrae automáticamente de la respuesta del login y se aplica a las demás solicitudes protegidas.
    3.  Ejecuta el `Login` cuando necesites un token fresco.

### 🔗 Descripción General de Endpoints

* **Base URL:** Todas las peticiones usan `http://localhost:5190/api` como base.

#### **Operaciones Comunes:**

* **CRUD Completo:** Los recursos de **Empleados (`/employee`)**, **Perfiles (`/profile`)** y **Usuarios (`/user`)** soportan operaciones CRUD completas:
    * `GET /getall`: Recupera una lista paginada y filtrada.
    * `GET /getbyid/{id}`: Recupera un recurso por su ID.
    * `POST /create`: Crea un nuevo recurso.
    * `PUT /update/{id}`: Actualiza un recurso existente.
    * `DELETE /delete/{id}`: Elimina un recurso.
* **Paginación y Filtrado:** Los endpoints `GET /getall` soportan parámetros genéricos de paginación (`QueryParams.PageNumber`, `QueryParams.PageSize`, `QueryParams.SortBy`, `QueryParams.Order`) y parámetros de filtrado específicos para cada recurso (ej. `FullName` para empleados, `Name` para perfiles, `Email` y `Role` para usuarios).
    * **Nota para Insomnia:** Los parámetros de ejemplo en las solicitudes `Get all` están deshabilitados por defecto en la colección. Habilítalos en la pestaña "Params" para probar la paginación y el filtrado.

#### **🔐 Autenticación Específica (`/auth`)**

* **`POST /auth/login`**: Autentica a un usuario y devuelve un JWT.
* **`POST /auth/register`**: Registra un nuevo usuario (protegido por rol `Admin`).

---

## 🌟 Próximas Mejoras

* ✅ Implementación de pruebas unitarias y de integración.
* Implementar pruebas de integración para los casos de uso de la API.
* Implementar pruebas unitarias para `UserService`.

## 📝 Autoría

Este proyecto ha sido desarrollado por:

* **Keyner de Ávila** - [LinkedIn](https://www.linkedin.com/in/kdeavila9/) - [Portfolio](https://kdeavila.site)
