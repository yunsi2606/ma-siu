# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/PostService/PostService.Api/*.csproj src/Services/PostService/PostService.Api/
COPY src/Services/PostService/PostService.Domain/*.csproj src/Services/PostService/PostService.Domain/
COPY src/Services/PostService/PostService.Application/*.csproj src/Services/PostService/PostService.Application/
COPY src/Services/PostService/PostService.Infrastructure/*.csproj src/Services/PostService/PostService.Infrastructure/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/
COPY src/Shared/EventContracts/*.csproj src/Shared/EventContracts/

RUN dotnet restore src/Services/PostService/PostService.Api/PostService.Api.csproj

COPY src/Services/PostService/ src/Services/PostService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/PostService/PostService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PostService.Api.dll"]
