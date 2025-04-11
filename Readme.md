# Table of Content

- [Pre Requisite](#pre-requisite)
- [Manual Quick Start API](#manual-quick-start-api)
- [Seeding](#seeding)
- [Database ERD](#database-erd)
- [Important Note](#important-note)
- [Directory Structure](#directory-structure)

# Pre Requisite

- .Net sdk 8.0
- .Net Entity Framework
- MSSQL 2019
- NATs
- Redis

# VS Code Extensions

- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Github Copilot](https://marketplace.visualstudio.com/items?itemName=GitHub.copilot)
- [EditorConfig for VS Code](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig)

# Manual Quick Start API

- Install .Net Entity Framework : https://learn.microsoft.com/en-us/ef/core/get-started/overview/install
- Database
  - For x64 machine: Run DB MSSQL 2019 : `docker run --name mssqldock -e "ACCEPT_EULA=Y" -v sqldata:./mssqldata -e "MSSQL_SA_PASSWORD=Administrat@r123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest`
  - Fox arm machine (apple sillicon) : `docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=Rahasia_123" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1434:1434 -d --name=SQLServerEdge mcr.microsoft.com/azure-sql-edge`
- Run Redis (by docker : `docker run --name redisdock -p6379:6379 -d redis`)
- Run NATs (by docker : `docker run --name natsdock -p 4222:4222 -p 6222:6222 -p 8222:8222 -d nats`)
- Copy file `appsettings.example.json` ubah ke `appsettings.json` kemudian setting konfigurasinya `cp appsettings.example.json appsettings.json`
- Run `dotnet ef database update` untuk migrate database
- Run `dotnet run` dan `dotnet watch` untuk otomatis hot reload

# Seeding

- Run `dotnet run command --command seed` untuk migrate semua seeder dari database
- Run `dotnet run command --command seed --args <module>Seeder` untuk migrate satu seeder/module dari database
  - Example seeding User: `dotnet run command --command seed --args UserSeeder`
  - Example seeding All Data: `dotnet run command --command seed`
- Jika membuat seeder baru pastikan masukkan juga ke `/Commands/SeederCommand.cs` agar bisa melakukan seeding all table nantinya. Pastikan juga urutannya sesuai.

# Database ERD

- ERD https://dbdocs.io/devops.dot/dotnet-service-boilerplate

# Important Note

After clonning this boilerplate for project, don't forget to change this according your project name:

- DB Context Name
- `.csproj` Name
- `.sln` Name

# Directory Structure

```
├── Commands
├── Constants
│   ├── Cache
│   ├── CircuitBreaker
│   ├── Event
│   ├── Logger
│   └── Storage
├── Domain
│   ├── Auth
│   │   ├── Dto
│   │   ├── Repositories
│   │   └── Services
│   ├── Logging
│   │   ├── Dto
│   │   ├── Repositories
│   │   └── Services
│   ├── Permission
│   │   ├── Dto
│   │   ├── Repositories
│   │   └── Services
│   ├── Role
│   │   ├── Dto
│   │   ├── Repositories
│   │   └── Services
│   ├── RolePermission
│   │   ├── Dto
│   │   ├── Repositories
│   │   └── Services
│   └── User
│       ├── Dto
│       ├── Repositories
│       └── Services
├── Http
│   └── API
│       └── Version1
│           └── Controllers
│               ├── Auth
│               └── IAM
├── Immutables
├── Infrastructure
│   ├── BackgroundHosted
│   ├── Databases
│   ├── Events
│   ├── Exceptions
│   ├── Filters
│   ├── Helpers
│   ├── Integrations
│   │   ├── Http
│   │   └── NATs
│   ├── Middlewares
│   ├── Queues
|   ├── SeedersData
│   ├── Shareds
│   └── Subsribtions
├── Log
├── Migrations
├── Models
├── Properties
├── Types
└── storage
```
