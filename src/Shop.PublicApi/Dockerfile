FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base

# Update package lists
RUN apt-get update && apt-get install -y --no-install-recommends

WORKDIR /app

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Specifies whether .NET welcome and telemetry messages are displayed on the first run.
ENV DOTNET_NOLOGO=1

# Specifies whether data about the .NET tools usage is collected and sent to Microsoft.
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Defines the build configuration to use at build time. The default is 'Release'
ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/Shop.PublicApi/Shop.PublicApi.csproj", "src/Shop.PublicApi/"]
COPY ["src/Shop.Application/Shop.Application.csproj", "src/Shop.Application/"]
COPY ["src/Shop.Domain/Shop.Domain.csproj", "src/Shop.Domain/"]
COPY ["src/Shop.Core/Shop.Core.csproj", "src/Shop.Core/"]
COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
COPY ["src/Shop.Query/Shop.Query.csproj", "src/Shop.Query/"]
RUN dotnet restore "src/Shop.PublicApi/Shop.PublicApi.csproj"
COPY . .
WORKDIR "/src/src/Shop.PublicApi"
RUN dotnet build "Shop.PublicApi.csproj" --configuration $BUILD_CONFIGURATION --no-restore --nologo --output /app/build 

FROM build AS publish
RUN dotnet publish "Shop.PublicApi.csproj" --configuration $BUILD_CONFIGURATION --no-restore --nologo --output /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]