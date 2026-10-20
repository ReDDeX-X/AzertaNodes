FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/AzertaNodes.Api/AzertaNodes.Api.csproj", "src/AzertaNodes.Api/"]
COPY ["src/AzertaNodes.Application/AzertaNodes.Application.csproj", "src/AzertaNodes.Application/"]
COPY ["src/AzertaNodes.Core/AzertaNodes.Core.csproj", "src/AzertaNodes.Core/"]
COPY ["src/Infrastructure/AzertaNodes.Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/AzertaNodes.Api/AzertaNodes.Api.csproj"

COPY . .
WORKDIR "/src/src/AzertaNodes.Api"

RUN dotnet publish "AzertaNodes.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AzertaNodes.Api.dll"]