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
4.  Puedes usar herramientas como Postman, Insomnia o un navegador web para interactuar con la API.

## 🌐 Endpoints de la API

### 🔐 Autenticación (`/Auth`)

* **`POST /Auth/login`**
    * **Descripción:** Autentica a un usuario y devuelve un JSON Web Token (JWT) si las credenciales son válidas.
    * **Cuerpo de la Petición (`UserLoginDto`):**
        ```json
        {
          "email": "usuario@ejemplo.com",
          "password": "miContrasenaSegura123"
        }
        ```
    * **Códigos de Respuesta:** `200 OK` (con JWT), `400 Bad Request` (datos de entrada inválidos), `404 Not Found` (usuario no encontrado o credenciales inválidas).
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        {
          "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c3VhcmlvQGVqZW1wbG8uY29tIiwianRpIjoiZmRlY..."
        }
        ```

* **`POST /Auth/register`**
    * **Descripción:** Registra un nuevo usuario en el sistema. Protegido por el rol `Admin`.
    * **Cuerpo de la Petición (`UserRegisterDto`):**
        ```json
        {
          "email": "nuevo.usuario@ejemplo.com",
          "password": "otraContrasenaSegura456!",
          "role": "Viewer" // O "Manager", "Admin"
        }
        ```
    * **Códigos de Respuesta:** `200 OK`, `400 Bad Request` (errores de validación, rol inválido), `409 Conflict` (usuario ya existe), `401 Unauthorized` (si no es Admin).
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        "Registration successful"
        ```

### 🧑‍💻 Empleados (`/Employee`)

* **`GET /Employee/getall`**
    * **Descripción:** Recupera una lista de todos los empleados. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Código de Respuesta:** `200 OK`
    * **Ejemplo de Respuesta:**
        ```json
        [
            {
                "id": 1,
                "fullName": "Juan David Pérez",
                "salary": 5500000,
                "idProfile": 1,
                "nameProfile": "Desarrollador Frontend"
            }
            // ... más empleados
        ]
        ```

* **`GET /Employee/getbyid/{id}`**
    * **Descripción:** Recupera un único empleado por su ID. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Código de Respuesta:** `200 OK` (si se encuentra), `404 Not Found` (si no se encuentra)
    * **Ejemplo de Petición (para ID 1):** `GET /Employee/getbyid/1`
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        {
            "id": 1,
            "fullName": "Juan David Pérez",
            "salary": 5500000,
            "idProfile": 1,
            "nameProfile": "Desarrollador Frontend"
        }
        ```
    * **Ejemplo de Respuesta (404 Not Found):**
        ```json
        "Empleado con ID 99 no encontrado."
        ```

* **`POST /Employee`**
    * **Descripción:** Crea un nuevo empleado. Requiere autenticación con rol `Admin` o `Manager`.
    * **Cuerpo de la Petición (`EmployeeDto`):**
        ```json
        {
            "fullName": "Nuevo Empleado de Prueba",
            "salary": 4800000,
            "idProfile": 3
        }
        ```
    * **Código de Respuesta:** `201 Created` (si es exitoso), `400 Bad Request` (por errores de validación o `IdProfile` inválido), `401 Unauthorized`.
    * **Ejemplo de Respuesta (201 Created):**
        ```json
        {
            "id": 16, // ID generado automáticamente
            "fullName": "Nuevo Empleado de Prueba",
            "salary": 4800000,
            "idProfile": 3,
            "nameProfile": "Ingeniero QA"
        }
        ```
      *La cabecera `Location` de la respuesta indicará el URL del nuevo recurso creado, por ejemplo: `/Employee/getbyid/16`*

* **`PUT /Employee/{id}`**
    * **Descripción:** Actualiza un empleado existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición. Requiere autenticación con rol `Admin` o `Manager`.
    * **Cuerpo de la Petición (`EmployeeDto`):**
      *Ejemplo para actualizar el empleado con ID 1:*
        ```json
        {
            "id": 1,
            "fullName": "Juan David Pérez Actualizado",
            "salary": 5600000,
            "idProfile": 1
        }
        ```
    * **Código de Respuesta:** `200 OK` (si es exitoso), `400 Bad Request` (errores de validación, IDs no coinciden o `IdProfile` inválido), `404 Not Found` (si el empleado no existe), `401 Unauthorized`.
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        {
            "id": 1,
            "fullName": "Juan David Pérez Actualizado",
            "salary": 5600000,
            "idProfile": 1,
            "nameProfile": "Desarrollador Frontend"
        }
        ```

* **`DELETE /Employee/{id}`**
    * **Descripción:** Elimina un empleado por su ID. Requiere autenticación con rol `Admin` o `Manager`.
    * **Código de Respuesta:** `204 No Content` (si es exitoso), `404 Not Found` (si el empleado no existe), `401 Unauthorized`.
    * **Ejemplo de Respuesta (204 No Content):** (Cuerpo de la respuesta vacío)

### 👥 Perfiles (`/Profile`)

* **`GET /Profile/getall`**
    * **Descripción:** Recupera una lista de todos los perfiles. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Código de Respuesta:** `200 OK`
    * **Ejemplo de Respuesta:**
        ```json
        [
            {
                "id": 1,
                "name": "Desarrollador Frontend"
            },
            {
                "id": 2,
                "name": "Desarrollador Backend"
            }
            // ... más perfiles
        ]
        ```

* **`GET /Profile/getbyid/{id}`**
    * **Descripción:** Recupera un único perfil por su ID. Requiere autenticación con rol `Admin`, `Manager` o `Viewer`.
    * **Código de Respuesta:** `200 OK` (si se encuentra), `404 Not Found` (si no se encuentra)
    * **Ejemplo de Petición (para ID 1):** `GET /Profile/getbyid/1`
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        {
            "id": 1,
            "name": "Desarrollador Frontend"
        }
        ```
    * **Ejemplo de Respuesta (404 Not Found):**
        ```json
        "Profile not found!"
        ```

* **`POST /Profile/create`**
    * **Descripción:** Crea un nuevo perfil. Requiere autenticación con rol `Admin` o `Manager`.
    * **Cuerpo de la Petición (`ProfileDto`):**
        ```json
        {
            "name": "Nuevo Perfil"
        }
        ```
    * **Código de Respuesta:** `201 Created` (si es exitoso), `400 Bad Request` (errores de validación), `409 Conflict` (si el nombre ya existe), `401 Unauthorized`.
    * **Ejemplo de Respuesta (201 Created):**
        ```json
        {
            "id": 5, // ID generado automáticamente
            "name": "Nuevo Perfil"
        }
        ```
      *La cabecera `Location` de la respuesta indicará el URL del nuevo recurso creado, por ejemplo: `/api/Profile/getbyid/5`*
    * **Ejemplo de Respuesta (409 Conflict):**
        ```json
        "Profile with name 'Nuevo Perfil' already exists."
        ```

* **`PUT /Profile/update/{id}`**
    * **Descripción:** Actualiza un perfil existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición. Requiere autenticación con rol `Admin` o `Manager`.
    * **Cuerpo de la Petición (`ProfileDto`):**
      *Ejemplo para actualizar el perfil con ID 1:*
        ```json
        {
            "id": 1,
            "name": "Desarrollador Frontend Actualizado"
        }
        ```
    * **Código de Respuesta:** `200 OK` (si es exitoso), `400 Bad Request` (errores de validación, IDs no coinciden), `404 Not Found` (si el perfil no existe), `409 Conflict` (si el nuevo nombre ya existe para otro perfil), `401 Unauthorized`.
    * **Ejemplo de Respuesta (200 OK):**
        ```json
        {
            "id": 1,
            "name": "Desarrollador Frontend Actualizado"
        }
        ```
    * **Ejemplo de Respuesta (409 Conflict):**
        ```json
        "Profile with name 'Desarrollador Backend' already exists for another profile."
        ```

* **`DELETE /Profile/delete/{id}`**
    * **Descripción:** Elimina un perfil por su ID. Requiere autenticación con rol `Admin` o `Manager`.
    * **Código de Respuesta:** `204 No Content` (si es exitoso), `404 Not Found` (si el perfil no existe), `401 Unauthorized`.
    * **Ejemplo de Respuesta (204 No Content):** (Cuerpo de la respuesta vacío)

## 🌟 Próximas Mejoras

* Implementar paginación y filtrado más avanzado para los endpoints GET.
* Mejoras en el registro (logging) y manejo de errores.
* Implementación de pruebas unitarias y de integración.
* Documentación de API con Swagger/OpenAPI.
* **Gestión de Usuarios (para administradores):** Crear endpoints para que los usuarios con rol 'Admin' puedan listar, ver, editar y eliminar otras cuentas de usuario.

## 📝 Autoría

Este proyecto ha sido desarrollado por:

* **Keyner de Ávila** - [LinkedIn](https://www.linkedin.com/in/kdeavila9/) - [Portfolio](https://kdeavila.site)