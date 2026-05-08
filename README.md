# Students & Users Management API - .NET 8.0

## 🚀 Overview
A high-performance **RESTful Web API** designed with a scalable architecture to manage people and system users. This project demonstrates a deep understanding of **Database Design**, **Backend Security**, and the **3-Tier Architecture** pattern.

### 🏗 Architecture & Design Patterns
*   **3-Tier Architecture**: Strict separation between Presentation (Controllers), Logic (BLL), and Data (DAL).
*   **Relational Schema**: Transitioned from a flat "Students" table to a normalized relational **People/Users** schema.
*   **Pure ADO.NET**: Intentionally avoids heavy ORMs like Entity Framework to achieve maximum execution speed and total control over SQL execution.

---

## 🔐 Security Features
*   **BCrypt Password Hashing**: Implements industry-standard salting and hashing (Cost Factor: 11) for user credentials.
*   **T-SQL Protection**: 100% protection against SQL Injection by using **Stored Procedures** and parameterized queries.
*   **Clean Configuration**: Sensitive connection strings are managed via **Environment Variables** (following the 12-Factor App methodology).

---

## ⚡ Database Optimization (T-SQL)
*   **Computed Columns**: Implemented dynamic **Age calculation** in the `People` table to ensure data integrity and reduce application-level overhead.
*   **Business Logic in SPs**: Centralized data logic within SQL Stored Procedures for better performance and reusability (DRY Principle).
*   **Scalable Schema**: Supports multiple roles (`Admin`, `Student`) and linkable person profiles.

---

## 🛠 Tech Stack
*   **Framework**: .NET 8.0
*   **Database**: SQL Server 2022
*   **Data Access**: ADO.NET (Pure SQL)
*   **Security**: BCrypt.Net-Next
*   **Documentation**: Swagger/OpenAPI

---

## 📖 API Endpoints

### Users & Authentication
| Method | Endpoint | Description |
|:--- |:--- |:--- |
| GET | `/api/Users/All` | Retrieve all system users with full profiles. |
| GET | `/api/Users/{id}` | Get user details by ID. |
| POST | `/api/Users` | Register a new user (with BCrypt hashing). |
| PUT | `/api/Users/UpdatePassword` | Securely update user password. |

### People Management
| Method | Endpoint | Description |
|:--- |:--- |:--- |
| GET | `/api/People/All` | List all people (Calculates age automatically). |
| DELETE | `/api/People/{id}` | Remove person and linked records. |

---

## 🧪 Testing & Credentials
To test the **Ownership Checks** and **Role-based Access**, you can use the following pre-configured accounts in the Swagger UI:

| Role | Username | Password | Access Level |
|:--- |:--- |:--- |:--- |
| **Admin** | `Belal` | `1235` | Full access to all resources. |
| **Student** | `Ali` | `Ali1234` | Restricted to own data (Ownership Secured). |

**Note:** Ensure you use the `POST /api/Auth/login` (or your login endpoint) to receive the **Bearer Token** before testing secured routes.


## 🚀 Getting Started
1.  **Clone the repository.**
2.  **Database Setup**:
    *   Locate the `Database/StudentManagementDB_FullScript.sql` file.
    *   Execute the script in your SQL Server instance to recreate the schema, SPs, and sample data.
3.  **Environment Configuration**:
    *   Set an environment variable `STUDENT_DB_CONNECTION` with your connection string.
4.  **Run**:
    *   Hit `F5` in Visual Studio 2022 and explore the **Swagger UI**.