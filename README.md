# CRUD API de Gestión de Empleados con .NET 9 y SQL Server

## ✨ Descripción del Proyecto

Este proyecto es una demostración de una API Web robusta y bien estructurada, construida con **ASP.NET Core (.NET 9)** y **Entity Framework Core**, diseñada para la gestión de datos de empleados. El objetivo principal es mostrar la implementación de principios de **Clean Architecture**, una clara **separación de responsabilidades** entre capas (Controladores, Servicios, Entidades, DTOs) y una **validación de datos exhaustiva** en cada nivel.

La API proporciona una base sólida para el desarrollo de aplicaciones backend escalables, mantenibles y fáciles de testear.

## 🚀 Características Clave

* **Arquitectura por Capas:** Separación clara entre la capa de presentación (Controladores), la lógica de negocio (Servicios) y el acceso a datos (Entidades, DbContext).
* **Validación de Entrada Robusta:**
    * Utiliza `Data Annotations` en los DTOs para la validación automática del modelo.
    * Implementa validaciones de reglas de negocio específicas dentro de la capa de Servicios (ej. verificar la existencia de claves foráneas).
* **Respuestas API Explícitas:** Emplea objetos de resultado personalizados (`ServiceResultStatus`) desde la capa de servicio para habilitar respuestas HTTP precisas y significativas (ej. `200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`).
* **Endpoints RESTful:** Adhiere a los principios RESTful para las operaciones CRUD (Crear, Leer, Actualizar, Borrar) de recursos.
* **Eficiencia en Acceso a Datos:** Aprovecha Entity Framework Core para las interacciones con la base de datos, incluyendo carga anticipada (`.Include()`) y proyecciones eficientes a DTOs (`.Select()`).
* **Integración con SQL Server:** Utiliza SQL Server como motor de base de datos relacional.

## 🛠️ Tecnologías Utilizadas

* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**
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

*(Actualmente implementados para la gestión de Empleados. Próximamente se añadirán los de Perfiles.)*

### 🧑‍💻 Empleados (`/Employee`)

* **`GET /Employee/getall`**
    * **Descripción:** Recupera una lista de todos los empleados.
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
    * **Descripción:** Recupera un único empleado por su ID.
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
    * **Descripción:** Crea un nuevo empleado.
    * **Cuerpo de la Petición (`EmployeeDto`):**
        ```json
        {
            "fullName": "Nuevo Empleado de Prueba",
            "salary": 4800000,
            "idProfile": 3
        }
        ```
    * **Código de Respuesta:** `201 Created` (si es exitoso), `400 Bad Request` (por errores de validación o `IdProfile` inválido)
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
    * **Descripción:** Actualiza un empleado existente. El `id` en la URL debe coincidir con el `Id` en el cuerpo de la petición.
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
    * **Código de Respuesta:** `200 OK` (si es exitoso), `400 Bad Request` (errores de validación, IDs no coinciden o `IdProfile` inválido), `404 Not Found` (si el empleado no existe)
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
    * **Descripción:** Elimina un empleado por su ID.
    * **Código de Respuesta:** `204 No Content` (si es exitoso), `404 Not Found` (si el empleado no existe)
    * **Ejemplo de Respuesta (204 No Content):** (Cuerpo de la respuesta vacío)

## 🌟 Próximas Mejoras

* Implementación completa de las operaciones CRUD para la entidad **`Profile`**.
* Añadir autenticación y autorización.
* Implementar paginación y filtrado más avanzado para los endpoints GET.
* Mejoras en el registro (logging) y manejo de errores.
* Implementación de pruebas unitarias y de integración.
* Documentación de API con Swagger/OpenAPI.

## 📝 Autoría

Este proyecto ha sido desarrollado por:

* **Keyner de Ávila** - [LinkedIn](https://www.linkedin.com/in/kdeavila9/) - [Portfolio](https://kdeavila.site)
