# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/AffiliateService/AffiliateService.Api/*.csproj src/Services/AffiliateService/AffiliateService.Api/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/

RUN dotnet restore src/Services/AffiliateService/AffiliateService.Api/AffiliateService.Api.csproj

COPY src/Services/AffiliateService/ src/Services/AffiliateService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/AffiliateService/AffiliateService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
EXPOSE 8205
ENV ASPNETCORE_URLS=http://+:8205
ENTRYPOINT ["dotnet", "AffiliateService.Api.dll"]
