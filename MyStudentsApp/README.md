# Students API - .NET 8.0 Web API

## Overview
A RESTful Web API developed using **ASP.NET Core 8.0** to manage student records. The API interacts with a SQL Server database through **Stored Procedures (SPs)** for CRUD operations.

## Key Technologies
- **ASP.NET Core 8.0**: For building the Web API.
- **SQL Server**: Data stored and manipulated via **Stored Procedures**.
- **Swagger**: For API documentation and testing.
- **Microsoft.Data.SqlClient**: For database connection.

## API Endpoints
- **GET /api/Students/All**: Get all students.
- **GET /api/Students/{id}**: Get student by ID.
- **POST /api/Students**: Add new student.
- **PUT /api/Students/{id}**: Update student.
- **DELETE /api/Students/{id}**: Delete student by ID.

## Purpose
This project demonstrates backend API development skills with a focus on data validation, performance (using SPs), and clean API design.