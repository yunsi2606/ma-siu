# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/ApiGateway/*.csproj src/ApiGateway/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/

RUN dotnet restore src/ApiGateway/ApiGateway.csproj

COPY src/ApiGateway/ src/ApiGateway/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/ApiGateway
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8300/health || exit 1

EXPOSE 8300
ENV ASPNETCORE_URLS=http://+:8300

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
