FROM mcr.microsoft.com/dotnet/aspnet:7.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV DOTNET_NOLOGO=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

FROM mcr.microsoft.com/dotnet/sdk:7.0-bookworm-slim AS build
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
RUN dotnet build "Shop.PublicApi.csproj" --configuration Release --no-restore --nologo --output /app/build 

FROM build AS publish
RUN dotnet publish "Shop.PublicApi.csproj" --configuration Release --no-restore --nologo --self-contained true --output /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]