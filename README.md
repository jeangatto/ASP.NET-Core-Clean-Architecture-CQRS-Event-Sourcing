# ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture

[![wakatime](https://wakatime.com/badge/github/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID.svg)](https://wakatime.com/badge/github/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![License](https://img.shields.io/github/license/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID.svg)](LICENSE)

[![Build](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml)
[![SonarCloud](https://github.com/JeanGatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/sonar-cloud.yml/badge.svg)](https://github.com/JeanGatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/sonar-cloud.yml)
[![CodeQL](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml)
[![DevSkim](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=coverage)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=bugs)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=code_smells)](https://sonarcloud.io/dashboard?id=ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)

About the repoitory:
Open source project written in the latest version of ASP.NET Core, implementing the concepts of S.O.L.I.D, Clean Code,
CQRS (Command Query Responsibility Segregation)

## Give it a star! ⭐

If you liked this project, learned something, give it a star. Thank you!

## **Technologies**

- ASP.NET Core 7.0
- Entity Framework Core 7.0
- Unit & Integration Tests + xUnit + FluentAssertions
- Polly
- AutoMapper
- FluentValidator
- MediatR
- Swagger UI
- HealthChecks
- SQL Server
- MongoDB
- Redis (Cache)
- Docker & Docker Compose

## **Architecture**

![CQRS Pattern](https://raw.githubusercontent.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/main/img/cqrs-pattern.png "CQRS Pattern")

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
dotnet clean && dotnet build
```

Next step, run the command in the terminal:

```csharp
docker-compose up --build
```

Now just open the url in the browser:

```csharp
http://localhost:5072/swagger/
```

## MiniProfiler for .NET

To access the page with the performance indicators and performance:

```csharp
http://localhost:5072/profiler/results-index
```

## License

- [MIT License](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE)
