# 🏋️‍♂️ SportsCenter API

This is a backend project in development for managing reservations in a sports facility. The API is built with **ASP.NET Core** following clean architecture principles (Controllers → Services → Repositories), and implements **JWT authentication**, role and permission control, and a fully functional facility reservation system.

> ⚠️ **IMPORTANT:** This project is still under development. Key features yet to be completed include:
> - Advanced validation
> - Automated testing
> - Docker support
> - Cloud deployment (AWS planned)  
> Still, the app is **functional**.

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

---

## 📁 Project Structure

SportsCenterApi/
│
├── Controllers/
│ ├── AuthController.cs
│ ├── FacilitiesController.cs
│ ├── ReservationController.cs
│ └── UsersController.cs
│
├── Models/
│ ├── Entities/
│ ├── DTO/
│ └── Extensions/
│
├── Services/
│ ├── Interfaces/
│ └── Implementations/
│
├── Repositories/
│ ├── Interfaces/
│ └── Implementations/
│
├── Program.cs
├── appsettings.json
└── ...


---

## ✅ Main Endpoints

All endpoints are available and testable via Swagger (`/swagger`):

### 🔐 Authentication
- `POST /api/auth/login` → Returns JWT token

### 👤 Users
- `GET /api/users` → Get all users
- `POST /api/users` → Create new user
- `PUT /api/users/{id}` → Edit user
- `DELETE /api/users/{id}` → Delete user
- `POST /api/users/change-password` → Change own password
- `POST /api/users/{id}/change-password` → Change another user's password (with permissions)

### 🏟️ Facilities
- `GET /api/facilities` → List all facilities
- `GET /api/facilities/{id}` → Get facility by ID
- `POST /api/facilities` → Create facility (roles: `Facility Manager`, `Administrator`)
- `PUT /api/facilities/{id}` → Edit facility
- `DELETE /api/facilities/{id}` → Delete facility
- `GET /api/facilities/filter` → Filter by type, date, and time

### 📅 Reservations
- `GET /api/reservation` → List all reservations
- `GET /api/reservation/filter` → Filter by user and date range
- `POST /api/reservation` → Create a new reservation (with permission)
- `DELETE /api/reservation/{id}` → Cancel reservation (based on permission and rules)

### 🛡️ Role
- `GET /api/role` → Get all roles (Administrator only)
- `GET /api/role/{id}` → Get role by ID (Administrator only)
- `POST /api/role` → Create a new role (Administrator only)
- `PUT /api/role/{id}` → Update an existing role (Administrator only)
- `DELETE /api/role/{id}` → Delete a role (Administrator only)

---

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


## 📌 Current Status
+ ✅ Core features complete  
+ 🔐 Auth + role-based permissions working  
+ 🧪 xUnit tests implemented  
+ 📄 Swagger documentation enabled  
+ 🐳 Dockerization completed and running at `http://localhost:5000/swagger/index.html`  
+ ☁️ AWS deployment pending


## 📃 License
This code was developed by **Samuel Radu Dragomir** and is part of my personal portfolio.  
It is shared publicly for **demonstration and educational purposes only**.  
**Commercial use, redistribution, or repackaging is strictly prohibited.**

💼 Open to Work 🚀
This project is part of my professional portfolio. I'm actively looking for new opportunities as a backend or full-stack developer.
If you're hiring or interested in my work, feel free to reach out! 😊
