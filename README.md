# CRUD API con .NET 9 y SQL Server

## ✨ Descripción del Proyecto

Este proyecto es una demostración de una API Web robusta y bien estructurada, construida con **ASP.NET Core (.NET 9)** y **Entity Framework Core**, diseñada para la gestión de datos de empleados y perfiles. El objetivo principal es mostrar la implementación de principios de **Clean Architecture**, una clara **separación de responsabilidades** entre capas (Controladores, Servicios, Entidades, DTOs) y una **validación de datos exhaustiva** en cada nivel.

La API proporciona una base sólida para el desarrollo de aplicaciones backend escalables, mantenibles y fáciles de testear.

## 🚀 Características Clave

* **Arquitectura por Capas:** Separación clara entre la capa de presentación (Controladores), la lógica de negocio (Servicios) y el acceso a datos (Entidades, DbContext).
* **Validación de Entrada Robusta:**
    * Utiliza `Data Annotations` en los DTOs para la validación automática del modelo.
    * Implementa validaciones de reglas de negocio específicas dentro de la capa de Servicios (ej. verificar la existencia de claves foráneas o unicidad de nombres).
* **Respuestas API Explícitas:** Emplea objetos de resultado personalizados (`ServiceResultStatus`) desde la capa de servicio para habilitar respuestas HTTP precisas y significativas (ej. `200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`, `409 Conflict`).
* **Endpoints RESTful:** Adhiere a los principios RESTful para las operaciones CRUD (Crear, Leer, Actualizar, Borrar) de recursos.
* **Eficiencia en Acceso a Datos:** Aprovecha Entity Framework Core para las interacciones con la base de datos, incluyendo carga anticipada (`.Include()`) y proyecciones eficientes a DTOs (`.Select()`).
* **Integración con SQL Server:** Utiliza SQL Server como motor de base de datos relacional.
* **Paginación y Filtrado Avanzado:** Implementación de un sistema flexible de paginación y filtrado para endpoints de listado, utilizando DTOs genéricos y reutilizables.

## 🔑 Seguridad y Autenticación

* **Autenticación Basada en Tokens JWT:** Implementación de un flujo de autenticación seguro utilizando JSON Web Tokens (JWT) para verificar la identidad del usuario. Los nombres de los roles se manejan como strings para mayor legibilidad de la API.
* **Gestión Segura de Contraseñas:** Las contraseñas de los usuarios se almacenan de forma segura utilizando el algoritmo de hashing adaptativo **BCrypt**, que incluye salting automático para proteger contra ataques de fuerza bruta y tablas arcoíris.
* **Autorización Basada en Roles (RBAC):** Control de acceso granular a los endpoints de la API mediante roles de usuario definidos (`Admin`, `Manager`, `Viewer`). Los roles se asignan durante el registro y se validan con el atributo `[Authorize(Roles = "")]` en los controladores y métodos de acción.
* **Validación de Roles Robusta:** Se utiliza una validación personalizada en los DTOs para asegurar que los valores de rol proporcionados en las solicitudes correspondan a roles válidos y definidos en el sistema.

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

* Tu API requiere autenticación JWT para la mayoría de los endpoints protegidos. La colección de Insomnia ya está configurada para automatizar este proceso:
    1.  Abre la solicitud `POST /api/auth/login` dentro de la carpeta `Auth`.
    2.  Envía la solicitud con las credenciales de un usuario existente (ej. `admin@example.com` con su contraseña).
    3.  **Gestión Automática del Token:** Todas las demás solicitudes protegidas en las carpetas `Employees` y `Profiles` tienen configurado un **Bearer Token** que **extrae automáticamente el JWT** de la respuesta exitosa del login. Esto significa que no necesitas copiar y pegar el token manualmente cada vez que expire o inicies sesión.
    4.  Simplemente ejecuta el `Login` cuando necesites un token fresco, y las demás solicitudes lo usarán de forma transparente.

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

## 🌟 Próximas Mejoras

* Mejoras en el registro (logging) y manejo de errores.
* Implementación de pruebas unitarias y de integración.
* Documentación de API con Swagger/OpenAPI.
* **Gestión de Usuarios (para administradores):** Crear endpoints para que los usuarios con rol 'Admin' puedan listar, ver, editar y eliminar otras cuentas de usuario.

## 📝 Autoría

Este proyecto ha sido desarrollado por:

* **Keyner de Ávila** - [LinkedIn](https://www.linkedin.com/in/kdeavila9/) - [Portfolio](https://kdeavila.site)