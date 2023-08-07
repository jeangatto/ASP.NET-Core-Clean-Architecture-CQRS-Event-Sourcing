FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 5072

ENV ASPNETCORE_URLS=http://+:5072
ENV DOTNET_NOLOGO=true
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
ENV DOTNET_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /src
COPY ["Directory.Build.props", "src/"]
COPY ["Directory.Packages.props", "src/"]
COPY ["src/Shop.PublicApi/Shop.PublicApi.csproj", "src/Shop.PublicApi/"]
COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
COPY ["src/Shop.Domain/Shop.Domain.csproj", "src/Shop.Domain/"]
COPY ["src/Shop.Core/Shop.Core.csproj", "src/Shop.Core/"]
COPY ["src/Shop.Application/Shop.Application.csproj", "src/Shop.Application/"]
COPY ["src/Shop.Query/Shop.Query.csproj", "src/Shop.Query/"]
RUN dotnet restore "src/Shop.PublicApi/Shop.PublicApi.csproj"
COPY . .
WORKDIR "/src/src/Shop.PublicApi"
RUN dotnet build "Shop.PublicApi.csproj" --configuration Release --output /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Shop.PublicApi.csproj" --configuration Release --output /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]
