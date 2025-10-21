# 🏋️‍♂️ SportsCenter — Full Stack App (Angular + ASP.NET Core)

This is a full-stack web application for managing reservations in a sports facility.  
It includes a **frontend built with Angular** and a **backend API developed in ASP.NET Core**, following clean architecture principles.
The system supports **JWT authentication**, role and permission control, and a fully functional reservation flow with availability checks and filtering.

> ⚠️ **IMPORTANT:** This project is still under development.
---

## 🚀 Features

- JWT-based authentication
- Role and permission control (custom claims)
- User management (registration, editing, password change)
- Sports facility management (create, edit, delete, filter)
- Reservation system with availability checks and permission control
- Swagger documentation enabled for easy testing
- Decoupled architecture with dependency injection
- Layered structure: Controller → Service → Repository → DBContext

---

## 🛠️ Technologies

- [.NET 8+]
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT (Json Web Tokens)
- Swagger / Swashbuckle
- C#
- HTML, CSS, TypeScrypt(Angular)



## 🧩 Project Architecture

This project follows a clean, layered architecture in both backend and frontend, designed for scalability, maintainability, and ease of testing.

---

### 🔙 Backend — ASP.NET Core

The backend applies the **Controller → Service → Repository** pattern, with clear separation of concerns:

- **Controllers** handle HTTP requests and responses.
- **Services** contain business logic, abstracted through interfaces.
- **Repositories** manage data access using Entity Framework Core.
- **DTOs** are used to shape and secure API responses.
- **Interfaces** are implemented throughout to support dependency injection and unit testing.
- **Unit tests** are written using **xUnit** and **Moq**, allowing for isolated testing with mocked dependencies.

This structure promotes clean code, testability, and adherence to SOLID principles.

---

### 🌐 Frontend — Angular

The frontend is built with Angular and follows a modular, component-based design:

- **Components** manage UI and user interactions.
- **Services** handle API communication via `HttpClient`.
- **Models and DTOs** define structured data types for clarity and consistency.
- **Guards** protect routes based on user roles and permissions.(coming soon)
- **Interceptors** inject JWT tokens and handle errors globally.

This setup ensures a reactive, maintainable frontend with strong separation between logic and presentation.


## ✅ API Endpoints

All endpoints are available and testable via Swagger UI:

📍 `http://localhost:5000/swagger/index.html`

The API includes routes for:

- Authentication and JWT token management
- User registration, editing, and password control
- Role and permission management
- Facility creation, filtering, and availability checks
- Reservation creation, filtering, and cancellation

> For full details, please refer to the Swagger documentation.


## 🧪 How to Run

1. Clone the repository:
  git clone https://github.com/Nolagplss/ProyectoSports.git
cd ProyectoSports/ParteBackend

2. Set up your PostgreSQL database and update appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Host=host.docker.internal;Port=5432;Database=sports_center;Username=postgres;Password=postgresql"
}

3. Build the docker image: 
 docker build -t sportscenter-api .

4. Run the container: 
 docker run -d -p 5000:80 sportscenter-api

5. Visit Swagger UI in your browser: 
 http://localhost:5000/swagger/index.html

## Backend Structure
![1](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/BackendStructure.PNG) 
![2](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/BackendStructure2.PNG)
![3](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/BackendStructure3.PNG)

## 🌐 Frontend Status

The frontend is currently under development using **Angular**.  
At this stage, it includes only the **main dashboard layout** and the **login screen**.  
Further modules such as user management, reservation views, and facility interaction are planned for upcoming iterations.

## 🖼️ UI Screenshots

Here are some preview images of the current frontend in development:

### 🔐 Login Screen
![Login](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/Login.PNG)

### 🏠 Main Dashboard
![Main](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/Main.PNG)  
![Main2](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/Main2.PNG)

### 📅 Reservation Views
![Reservations](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/Reservations.PNG)  
![Reservations2](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/Reservations2.PNG)

### 🆕 New Reservation Flow
![NewReservation](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation.PNG)  
![NewReservation2](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation2.PNG)  
![NewReservation3](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation3.PNG)  
![NewReservation4](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation4.PNG)  
![NewReservation5](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation5.PNG)  
![NewReservation6](https://github.com/Nolagplss/ProyectoSports/blob/main/Assets/NewReservation6.PNG)


## 📌 Current Status
+ ✅ Core features complete  
+ 🔐 Auth + role-based permissions working  
+ 🧪 xUnit tests implemented  
+ 📄 Swagger documentation enabled  
+ 🐳 Dockerization completed and running at `http://localhost:5000/swagger/index.html`  
+ ☁️ AWS deployment pending
+ 🌐 Angular frontend in progress (currently includes login, main panel and new reservation only)

## 📃 License
This code was developed by **Samuel Radu Dragomir** and is part of my personal portfolio.  
It is shared publicly for **demonstration and educational purposes only**.  
**Commercial use, redistribution, or repackaging is strictly prohibited.**

💼 Open to Work 🚀
This project is part of my professional portfolio. I'm actively looking for new opportunities as a backend or full-stack developer.
If you're hiring or interested in my work, feel free to reach out! 😊
