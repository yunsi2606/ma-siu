# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY MaSiu.sln .
COPY src/Services/AuthService/AuthService.Api/*.csproj src/Services/AuthService/AuthService.Api/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/

# Restore
RUN dotnet restore src/Services/AuthService/AuthService.Api/AuthService.Api.csproj

# Copy source code
COPY src/Services/AuthService/ src/Services/AuthService/
COPY src/Shared/ src/Shared/

# Build
WORKDIR /src/src/Services/AuthService/AuthService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install health check tools
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
