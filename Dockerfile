# Imagen que se usa para ejecutar la API cuando ya esta publicada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Imagen que se usa para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copio primero la solucion y los proyectos para restaurar paquetes
COPY AgoraEspacios.sln .
COPY AgoraEspacios.API/AgoraEspacios.API.csproj AgoraEspacios.API/
COPY AgoraEspacios.Business/AgoraEspacios.Business.csproj AgoraEspacios.Business/
COPY AgoraEspacios.Data/AgoraEspacios.Data.csproj AgoraEspacios.Data/
COPY AgoraEspacios.Models/AgoraEspacios.Models.csproj AgoraEspacios.Models/

RUN dotnet restore AgoraEspacios.sln

# Copio el resto del codigo y publico la API
COPY . .
RUN dotnet publish AgoraEspacios.API/AgoraEspacios.API.csproj -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AgoraEspacios.API.dll"]
