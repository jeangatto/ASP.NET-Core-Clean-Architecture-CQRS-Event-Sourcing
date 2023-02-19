# ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, Princípios SOLID e Clean Architecture

[![Build](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/codeql-analysis.yml)
[![DevSkim](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml/badge.svg)](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/actions/workflows/devskim-analysis.yml)
[![License](https://img.shields.io/github/license/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID.svg)](LICENSE)

Sobre o respoitório:
Projeto de código aberto escrito na última versão do ASP.NET Core, implementando os conceitos do S.O.L.I.D, Clean Code, CQRS (Command Query Responsibility Segregation)

## Dê uma estrela! ⭐

Se você gostou deste projeto, aprendeu algo ou está usando em seus aplicativos, dê uma estrelinha. Obrigado!

![CQRS Pattern](https://raw.githubusercontent.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/main/cqrs-pattern.png "CQRS Pattern")

## **Tecnologias**

* ASP.NET 7.0
* .NET Core Native DI
* Entity Framework Core 7.0
* MongoDB
* Polly
* AutoMapper
* FluentValidator
* MediatR
* Swagger UI
* HealthChecks
* Docker & Docker Compose

## **Arquitetura**

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

## License

* [MIT License](https://github.com/jeangatto/ASP.NET-Core-API-CQRS-EVENT-DDD-SOLID/blob/main/LICENSE)
