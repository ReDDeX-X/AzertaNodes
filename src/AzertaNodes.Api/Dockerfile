FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AzertaNodes.Api/AzertaNodes.Api.csproj", "AzertaNodes.Api/"]
RUN dotnet restore "./AzertaNodes.Api/AzertaNodes.Api.csproj"
COPY . .
WORKDIR "/src/AzertaNodes.Api"
RUN dotnet build "./AzertaNodes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AzertaNodes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AzertaNodes.Api.dll"]