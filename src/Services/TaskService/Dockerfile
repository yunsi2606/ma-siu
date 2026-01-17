# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/TaskService/TaskService/*.csproj src/Services/TaskService/TaskService/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/
COPY src/Shared/Protos/*.csproj src/Shared/Protos/
COPY src/Shared/EventContracts/*.csproj src/Shared/EventContracts/

RUN dotnet restore src/Services/TaskService/TaskService/TaskService.csproj

COPY src/Services/TaskService/ src/Services/TaskService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/TaskService/TaskService
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
EXPOSE 8208
ENV ASPNETCORE_URLS=http://+:8208
ENTRYPOINT ["dotnet", "TaskService.dll"]
