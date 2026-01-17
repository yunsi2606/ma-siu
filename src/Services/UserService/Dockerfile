# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/UserService/UserService.Api/*.csproj src/Services/UserService/UserService.Api/
COPY src/Services/UserService/UserService.Domain/*.csproj src/Services/UserService/UserService.Domain/
COPY src/Services/UserService/UserService.Application/*.csproj src/Services/UserService/UserService.Application/
COPY src/Services/UserService/UserService.Infrastructure/*.csproj src/Services/UserService/UserService.Infrastructure/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/
COPY src/Shared/Protos/*.csproj src/Shared/Protos/

RUN dotnet restore src/Services/UserService/UserService.Api/UserService.Api.csproj

COPY src/Services/UserService/ src/Services/UserService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/UserService/UserService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
EXPOSE 8202
ENV ASPNETCORE_URLS=http://+:8202
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
