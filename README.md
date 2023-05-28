# ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, Princípios SOLID e Clean Architecture

[![Build](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml)
[![DevSkim](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml)
[![wakatime](https://wakatime.com/badge/github/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID.svg)](https://wakatime.com/badge/github/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![License](https://img.shields.io/github/license/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID.svg)](LICENSE)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=coverage)](https://sonarcloud.io/dashboard?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=bugs)](https://sonarcloud.io/dashboard?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID&metric=code_smells)](https://sonarcloud.io/dashboard?id=jeangatto_ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID)

Sobre o respoitório:
Projeto de código aberto escrito na última versão do ASP.NET Core, implementando os conceitos do S.O.L.I.D, Clean Code,
CQRS (Command Query Responsibility Segregation)

## Dê uma estrela! ⭐

Se você gostou deste projeto, aprendeu algo, dê uma estrelinha. Obrigado!

Criado com o [Rider: o IDE .NET de plataforma cruzada da JetBrains](https://www.jetbrains.com/pt-br/rider/)

![Rider logo](https://resources.jetbrains.com/storage/products/company/brand/logos/Rider_icon.svg)

## **Tecnologias**

* ASP.NET Core 7.0
* Entity Framework Core 7.0
* SQL Server
* MongoDB
* Redis (Cache)
* Polly
* AutoMapper
* FluentValidator
* MediatR
* Swagger UI
* HealthChecks
* Docker & Docker Compose

## **Arquitetura**

![CQRS Pattern](https://raw.githubusercontent.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/main/img/cqrs-pattern.png "CQRS Pattern")

* Full architecture with responsibility separation concerns, SOLID and Clean Code
* Domain Driven Design (Layers and Domain Model Pattern)
* Domain Events
* Domain Notification
* Domain Validations
* CQRS
* Event Sourcing
* Unit of Work
* Repository Pattern
* Resut Pattern

## Executando a aplicação

Após clonar o repositório na pasta desejada, executar o comando no terminal na raiz do projeto:

```csharp
dotnet clean && dotnet build
```

Próximo passo, executar o comando no terminal:

```csharp
docker-compose up --build
```

Agora basta abrir a url no navegador:

```csharp
http://localhost:5072/swagger/
```

## MiniProfiler for .NET

Para acessar a página com os indicadores de desempenho e performance:

```csharp
http://localhost:5072/profiler/results-index
```

## License

* [MIT License](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE)
