# ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture

[![Build](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/dotnet.yml)
[![SonarCloud](https://github.com/JeanGatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/sonar-cloud.yml/badge.svg)](https://github.com/JeanGatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/sonar-cloud.yml)
[![CodeQL](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/codeql-analysis.yml)
[![DevSkim](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/devskim-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/actions/workflows/devskim-analysis.yml)
[![License](https://img.shields.io/github/license/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing.svg)](LICENSE)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=coverage)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=bugs)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing&metric=code_smells)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)

[![Sparkline](https://stars.medv.io/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing.svg)](https://stars.medv.io/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)

About the repoitory:
Open source project written in the latest version of ASP.NET Core, implementing the concepts of S.O.L.I.D, Clean Code,
CQRS (Command Query Responsibility Segregation)

## Give it a star! ⭐

If you liked this project, learned something, give it a star. Thank you!

## **Technologies**

- ~~ASP.NET Core 8~~
- ASP.NET Core 9
- ~~Entity Framework Core 8~~
- Entity Framework Core 9
- **EF Compiled Queries** (https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/compiled-queries-linq-to-entities)
- Unit & Integration Tests + xUnit + FluentAssertions (7.1.0)
- Polly
- AutoMapper (15)
- FluentValidator
- MediatR (13)
- ~~Swagger UI~~
- OpenApi
- **Scalar** - Interactive API Reference from OpenAPI/Swagger (https://github.com/scalar/scalar)
- HealthChecks
- SQL Server
- MongoDB
- Redis (Cache)
- Docker & Docker Compose

## **Architecture**

![CQRS Pattern](img/cqrs-pattern.png "CQRS Pattern")

- Full architecture with responsibility separation concerns, SOLID and Clean Code
- Domain Driven Design (Layers and Domain Model Pattern)
- Domain Events
- Domain Notification
- Domain Validations
- CQRS
- Event Sourcing
- Unit of Work
- Repository Pattern
- Resut Pattern

## Running the application

After cloning the repository to the desired folder, run the command in the terminal at the root of the project:

```csharp
dotnet clean Shop.sln --nologo /tl && dotnet build Shop.sln --nologo /tl
```

Next step, run the command in the terminal:

```csharp
docker-compose up --build --abort-on-container-exit --remove-orphans
```

Now just open the url in the browser:

```csharp
http://localhost:{port}/scalar/v1
```

## MiniProfiler for .NET

To access the page with the performance indicators and performance:

```csharp
http://localhost:{port}/profiler/results-index
```

## License

- [MIT License](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing/blob/main/LICENSE)
