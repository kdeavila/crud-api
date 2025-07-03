# CRUD API con .NET 9 y SQL Server

## ✨ Descripción del Proyecto

Este proyecto demuestra una API Web robusta y estructurada con **ASP.NET Core (.NET 9)** y **Entity Framework Core**, enfocada en la gestión de empleados, perfiles y usuarios. Implementa **Clean Architecture**, **separación de responsabilidades** (Controladores, Servicios, Entidades, DTOs) y **validación de datos exhaustiva**, proporcionando una base escalable y mantenible.

## 🚀 Características Clave

* **Arquitectura por Capas:** Separación clara entre la presentación (Controladores), lógica de negocio (Servicios) y acceso a datos (EF Core).
* **Validación de Entrada Robusta:** Con `Data Annotations` en DTOs y validaciones de reglas de negocio en Servicios (ej. unicidad, claves foráneas).
* **Respuestas API Explícitas:** Usa `ServiceResultStatus` para respuestas HTTP precisas (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `409 Conflict`).
* **Endpoints RESTful:** Adhiere a los principios RESTful para las operaciones CRUD (Crear, Leer, Actualizar, Borrar) de recursos.
* **Eficiencia en Acceso a Datos:** Entity Framework Core para interacciones optimizadas (ej. `.Include()`, `.Select()`).
* **Integración con SQL Server:** Utiliza SQL Server como motor de base de datos relacional.
* **Paginación y Filtrado Avanzado:** Implementación de un sistema flexible de paginación y filtrado para endpoints de listado, utilizando DTOs genéricos y reutilizables.

## 🔑 Seguridad y Autenticación

* **Autenticación Basada en Tokens JWT:** Flujo de autenticación seguro con JWT, manejando roles como strings para legibilidad.
* **Gestión Segura de Contraseñas:** Contraseñas almacenadas con hashing **BCrypt** (salting automático) para protección contra ataques.
* **Autorización Basada en Roles (RBAC):** Control de acceso granular a endpoints mediante roles (`Admin`, `Manager`, `Viewer`), asignados en el registro y validados con `[Authorize(Roles = "")]`.
* **Validación de Roles Robusta:** Se utiliza una validación personalizada en los DTOs para asegurar que los valores de rol proporcionados correspondan a roles válidos.

## 🛠️ Tecnologías Utilizadas

* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
* **BCrypt.Net-Core** (para hashing de contraseñas)
* **DBeaver** (o SQL Server Management Studio para gestión de BD)

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
    * Ejecuta los comandos de Entity Framework Core para crear el esquema de la base de datos:
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

Para facilitar la interacción con la API, se proporciona una colección de Insomnia preconfigurada.

### 📥 Importar la Colección de Insomnia

1.  Abre Insomnia.
2.  Ve a `File > Import > From File` (o `Archivo > Importar > Desde Archivo`).
3.  Selecciona el archivo `Insomnia.yaml` que se encuentra en la raíz de este repositorio.
4.  Esto importará la colección "CRUD API" con sus carpetas (`Auth`, `Employees`, `Profiles`) y todas las solicitudes preconfiguradas.

### 🔑 Autenticación y Gestión de Tokens

* Tu API requiere autenticación JWT para la mayoría de los endpoints. La colección de Insomnia automatiza la gestión de tokens:
    1.  Realiza un `POST` a `/api/auth/login` con credenciales de usuario.
    2.  El **Bearer Token** se extrae automáticamente de la respuesta del login y se aplica a las demás solicitudes protegidas (`Employees`, `Profiles`, `Users`).
    3.  Ejecuta el `Login` cuando necesites un token fresco.

### 🔗 Descripción General de Endpoints

* **Base URL:** Todas las peticiones usan `http://localhost:5190/api` como base.

#### 🔐 Autenticación (`/Auth`)

* **`POST /auth/login`**
    * **Descripción:** Autentica a un usuario y devuelve un JSON Web Token (JWT).
    * **Cuerpo de la Petición (`UserLoginDto`):** `email`, `password`.
* **`POST /auth/register`**
    * **Descripción:** Registra un nuevo usuario en el sistema. Protegido por el rol `Admin`.
    * **Cuerpo de la Petición (`UserRegisterDto`):** `email`, `password`, `role` (`Viewer`, `Manager`, `Admin`).

#### 🧑‍💻 Empleados (`/Employee`)

* **`GET /employee/getall`**
    * **Descripción:** Recupera una lista paginada y filtrada de empleados. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Parámetros de Paginación/Ordenamiento (genéricos - `QueryParamsDto`):**
        * `QueryParams.PageNumber`: Número de página (ej. `1`, `2`).
        * `QueryParams.PageSize`: Cantidad de ítems por página (ej. `10`, `15`).
        * `QueryParams.SortBy`: Campo por el cual ordenar (ej. `id`, `fullName`, `salary`, `profile`).
        * `QueryParams.Order`: Orden de la paginación (`asc` para ascendente, `desc` para descendente).
    * **Parámetros de Filtrado (específicos de empleado):**
        * `FullName`: Filtra por el nombre completo del empleado.
        * `MinSalary`: Salario mínimo.
        * `MaxSalary`: Salario máximo.
        * `IdProfile`: ID del perfil asociado.
    * **Importante para Insomnia:** Los parámetros de ejemplo en la solicitud `Get all` de Insomnia están `disabled` (deshabilitados) por defecto en la colección importada. Deberás habilitarlos en la pestaña "Params" y ajustar sus valores para probar la paginación y el filtrado.
* **`GET /employee/getbyid/{id}`**
    * **Descripción:** Recupera un empleado por su ID. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
* **`POST /employee/create`**
    * **Descripción:** Crea un nuevo empleado. Requiere autenticación con rol `Admin` o `Manager`.
* **`PUT /employee/update/{id}`**
    * **Descripción:** Actualiza un empleado existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición. Requiere autenticación con rol `Admin` o `Manager`.
* **`DELETE /employee/delete/{id}`**
    * **Descripción:** Elimina un empleado por su ID. Requiere autenticación con rol `Admin` o `Manager`.

#### 👥 Perfiles (`/Profile`)

* **`GET /profile/getall`**
    * **Descripción:** Recupera una lista paginada y filtrada de perfiles. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Parámetros de Paginación/Ordenamiento (genéricos - `QueryParamsDto`):**
        * `QueryParams.PageNumber`, `QueryParams.PageSize`, `QueryParams.SortBy` (ej. `id`, `name`), `QueryParams.Order`.
    * **Parámetros de Filtrado (específicos de perfil):**
        * `Name`: Filtra por el nombre del perfil.
    * **Importante para Insomnia:** Los parámetros de ejemplo en la solicitud `Get all` de Insomnia están `disabled` (deshabilitados) por defecto en la colección importada. Deberás habilitarlos en la pestaña "Params" y ajustar sus valores para probar la paginación y el filtrado.
* **`GET /profile/getbyid/{id}`**
    * **Descripción:** Recupera un perfil por su ID. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
* **`POST /profile/create`**
    * **Descripción:** Crea un nuevo perfil. Requiere autenticación con rol `Admin` o `Manager`.
* **`PUT /profile/update/{id}`**
    * **Descripción:** Actualiza un perfil existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición. Requiere autenticación con rol `Admin` o `Manager`.
* **`DELETE /profile/delete/{id}`**
    * **Descripción:** Elimina un perfil por su ID. Requiere autenticación con rol `Admin` o `Manager`.

#### 👤 Usuarios (`/User`)

* **`GET /user/getall`**
    * **Descripción:** Recupera una lista paginada y filtrada de usuarios. Requiere autenticación con rol `Admin`.
    * **Parámetros de Paginación/Ordenamiento (genéricos - `QueryParamsDto`):**
        * `QueryParams.PageNumber`, `QueryParams.PageSize`, `QueryParams.SortBy` (ej. `id`, `email`, `role`), `QueryParams.Order`.
    * **Parámetros de Filtrado (específicos de usuario):**
        * `Email`: Filtra por el email del usuario.
        * `Role`: Filtra por el rol del usuario (`Viewer`, `Manager`, `Admin`).
* **`GET /user/getbyid/{id}`**
    * **Descripción:** Recupera un usuario por su ID. Requiere autenticación con rol `Admin`.
* **`PUT /user/update/{id}`**
    * **Descripción:** Actualiza un usuario existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición. Requiere autenticación con rol `Admin`.
* **`DELETE /user/delete/{id}`**
    * **Descripción:** Elimina un usuario por su ID. Requiere autenticación con rol `Admin`.

## 🌟 Próximas Mejoras

* Mejoras en el registro (logging) y manejo de errores.
* Implementación de pruebas unitarias y de integración.
* Documentación de API con Swagger/OpenAPI.

## 📝 Autoría

Este proyecto ha sido desarrollado por:

* **Keyner de Ávila** - [LinkedIn](https://www.linkedin.com/in/kdeavila9/) - [Portfolio](https://kdeavila.site)