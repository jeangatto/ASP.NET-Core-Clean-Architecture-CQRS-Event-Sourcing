FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5072

ENV ASPNETCORE_URLS=http://+:5072

# Disable .NET Core welcome and telemetry messages
ENV DOTNET_NOLOGO=true

# Disable .NET Core SDK telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Shop.PublicApi/Shop.PublicApi.csproj", "src/Shop.PublicApi/"]
COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
COPY ["src/Shop.Domain/Shop.Domain.csproj", "src/Shop.Domain/"]
COPY ["src/Shop.Core/Shop.Core.csproj", "src/Shop.Core/"]
COPY ["src/Shop.Application/Shop.Application.csproj", "src/Shop.Application/"]
RUN dotnet restore "src/Shop.PublicApi/Shop.PublicApi.csproj"
COPY . .
WORKDIR "/src/src/Shop.PublicApi"
RUN dotnet build "Shop.PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shop.PublicApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]
