# Employee Leave Management System (ELMS)

A simple **Employee Leave Management System** built with **ASP.NET Core MVC, Dapper, and SQL Server**.

The application supports two roles:

- **Employee** – apply for leave, check leave balance, and view leave history.
- **Admin** – manage employees, review leave requests, and approve/reject applications.

## Features

- Cookie-based authentication with role-based authorization
- Employee and Admin dashboards
- Leave application and history
- Leave balance and quota management
- Date and overlapping-leave validation
- Admin approval/rejection with remarks
- Employee management
- Pagination
- Audit logging
- SQL Server stored procedures
- Dapper-based data access

## Tech Stack

- ASP.NET Core 8.0 MVC
- C#
- SQL Server
- Dapper
- Razor Views
- Bootstrap 5
- FontAwesome

## Setup

### 1. Database

Open `EmpLMS_Database_Setup.sql` in **SQL Server Management Studio** and execute it.

This creates the `EmpLMS_DB` database, required tables, stored procedures, and test users.

### 2. Connection String

Update the connection string in `appsettings.json` if required:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmpLMS_DB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Run

Open `EmpLMS.sln` in **Visual Studio 2022** and press **F5**.

Or use:

```bash
dotnet build
dotnet run --launch-profile "http"
```

Application URL:

```text
http://localhost:5165
```

## Test Login

**Admin**

```text
Email: admin@example.com
Password: admin123
```

**Employee**

```text
Email: employee@example.com
Password: emp123
```

> Test credentials are included for local development only.