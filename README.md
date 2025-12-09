# Students API - .NET 8.0 Web API

## Overview
A **RESTful Web API** for managing student records using **3-Tier Architecture**:  
- **Presentation Layer**: API controllers  
- **Business Layer**: Logic & validation  
- **Data Access Layer**: SQL Server via **Stored Procedures (T-SQL) & ADO.NET**  

No Entity Framework is used — all database operations are **directly handled via SQL & ADO.NET**.

---

## Key Technologies
- **ASP.NET Core 8.0**  
- **SQL Server (Stored Procedures / T-SQL)**  
- **ADO.NET**  
- **Swagger** (API testing & documentation)  

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

## Purpose
Demonstrates **backend development skills** with:  
- Clean **3-Tier architecture**  
- Efficient **SQL Server operations using Stored Procedures**  
- Full **ADO.NET integration**  
- **RESTful API design** ready for real-world applications
