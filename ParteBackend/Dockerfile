FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Establecer variable de entorno para evitar problemas de NuGet
ENV NUGET_PACKAGES=/nuget
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Copiar solo el proyecto principal primero
COPY SportsCenterApi/ ./SportsCenterApi/

# Ir al directorio del proyecto y hacer todo el proceso
WORKDIR /src/SportsCenterApi

# Limpiar cualquier configuración previa
RUN dotnet nuget locals all --clear

# Restaurar y publicar en un solo paso
RUN dotnet publish SportsCenterApi.csproj -c Release -o /app/publish --self-contained false

# Imagen runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

EXPOSE 5000

# Copiar archivos publicados
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SportsCenterApi.dll"]