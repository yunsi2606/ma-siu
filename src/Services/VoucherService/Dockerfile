# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY MaSiu.sln .
COPY src/Services/VoucherService/VoucherService.Api/*.csproj src/Services/VoucherService/VoucherService.Api/
COPY src/Services/VoucherService/VoucherService.Domain/*.csproj src/Services/VoucherService/VoucherService.Domain/
COPY src/Services/VoucherService/VoucherService.Application/*.csproj src/Services/VoucherService/VoucherService.Application/
COPY src/Services/VoucherService/VoucherService.Infrastructure/*.csproj src/Services/VoucherService/VoucherService.Infrastructure/
COPY src/Shared/BuildingBlocks/*.csproj src/Shared/BuildingBlocks/
COPY src/Shared/EventContracts/*.csproj src/Shared/EventContracts/

RUN dotnet restore src/Services/VoucherService/VoucherService.Api/VoucherService.Api.csproj

COPY src/Services/VoucherService/ src/Services/VoucherService/
COPY src/Shared/ src/Shared/

WORKDIR /src/src/Services/VoucherService/VoucherService.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8204
ENV ASPNETCORE_URLS=http://+:8204

ENTRYPOINT ["dotnet", "VoucherService.Api.dll"]
