# ETAPA 1: Base - Imagen ligera para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# ETAPA 2: Build - SDK completo para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar todos los .csproj para restaurar dependencias
COPY ["TravelRequest/TravelRequest.csproj", "TravelRequest/"]
COPY ["TravelRequests.Application/TravelRequests.Application.csproj", "TravelRequests.Application/"]
COPY ["TravelRequests.Domain/TravelRequests.Domain.csproj", "TravelRequests.Domain/"]
COPY ["TravelRequests.Infrastructure/TravelRequests.Infrastructure.csproj", "TravelRequests.Infrastructure/"]

# Restaurar dependencias (cacheado si no cambian .csproj)
RUN dotnet restore "TravelRequest/TravelRequest.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar proyecto
WORKDIR "/src/TravelRequest"
RUN dotnet build "TravelRequest.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ETAPA 3: Publish - Generar archivos optimizados
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TravelRequest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ETAPA 4: Final - Imagen pequeña solo con lo necesario
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "TravelRequest.dll"]
