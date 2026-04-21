# Students API - .NET 8.0 Web API

## Overview
A **RESTful Web API** for managing student records built with a focus on **Performance** and **Security**. This project implements a **3-Tier Architecture** to ensure separation of concerns and maintainability.

### Architecture
- **Presentation Layer**: ASP.NET Core Web API controllers handling HTTP requests.
- **Business Logic Layer (BLL)**: Managing data validation and business rules.
- **Data Access Layer (DAL)**: High-performance database operations using **ADO.NET**.

> [!IMPORTANT]
> **Performance Note**: This project intentionally avoids heavy ORMs like Entity Framework, using **Pure ADO.NET and Stored Procedures (T-SQL)** for maximum control and efficiency.

---

## Key Features & Security
- **Secure Configuration**: Database connection strings are managed via **Environment Variables** to prevent sensitive data leaks.
- **SQL Injection Protection**: All database interactions are handled via **Stored Procedures** and Parameters.
- **Clean API Design**: Follows RESTful conventions for intuitive integration.
- **Modern Tech Stack**: Built on **.NET 8.0**.

---

## API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET    | `/api/Students/All` | Retrieve all students |
| GET    | `/api/Students/{id}` | Retrieve student by ID |
| POST   | `/api/Students` | Add new student |
| PUT    | `/api/Students/{id}` | Update student |
| DELETE | `/api/Students/{id}` | Delete student by ID |

---

## Getting Started
1. Clone the repository.
2. Set up your **Environment Variable**:
   - Variable Name: `STUDENT_DB_CONNECTION`
   - Value: `Your_SQL_Server_Connection_String`
3. Run the SQL script (provided in `/Database` folder) to create Stored Procedures.
4. Run the project and explore via **Swagger**.