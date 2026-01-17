# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/NotificationService/NotificationService.Api/*.csproj src/Services/NotificationService/NotificationService.Api/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/
COPY src/Shared/Protos/*.csproj src/Shared/Protos/
COPY src/Shared/EventContracts/*.csproj src/Shared/EventContracts/

RUN dotnet restore src/Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj

COPY src/Services/NotificationService/ src/Services/NotificationService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/NotificationService/NotificationService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
EXPOSE 8206
ENV ASPNETCORE_URLS=http://+:8206
ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
