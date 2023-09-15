# https://hub.docker.com/_/microsoft-dotnet-sdk/ 
ARG sdk_version=7.0-bookworm-slim 

FROM mcr.microsoft.com/dotnet/aspnet:$sdk_version AS base
WORKDIR /app
EXPOSE 5072

ENV ASPNETCORE_URLS=http://+:5072
ENV DOTNET_NOLOGO=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:$sdk_version AS build
ARG configuration=Release
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
RUN dotnet build "Shop.PublicApi.csproj" --configuration $configuration --no-restore --nologo --output /app/build 

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Shop.PublicApi.csproj" --configuration $configuration --no-restore --nologo --self-contained true --output /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]
