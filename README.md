# Mã Siu

> **Mã Siu**

Production-grade Voucher & Affiliate Notification Platform.

## Architecture

- **API Gateway**: YARP-based gateway at `gateway-sale.nhatcuong.io.vn`
- **Admin Dashboard**: Flutter Web at `admin-sale.nhatcuong.io.vn`
- **Mobile App**: Flutter for iOS/Android
- **Backend**: .NET 9 microservices
- **Database**: MongoDB
- **Cache**: Redis
- **Message Queue**: RabbitMQ
- **Object Storage**: MinIO

## Project Structure

```
ma-siu/
├── assets/                     # Static assets
├── config/                     # Configuration files
├── src/
│   ├── ApiGateway/             # YARP API Gateway
│   ├── Apps/
│   │   ├── App.Client/         # Flutter Mobile App
│   │   └── App.Admin/          # Flutter Web Admin
│   ├── Services/
│   │   ├── AuthService/        # Google OAuth + JWT
│   │   ├── UserService/        # User profiles
│   │   ├── PostService/        # Deal posts
│   │   ├── VoucherService/     # Voucher management
│   │   ├── AffiliateService/   # Affiliate links
│   │   ├── NotificationService/# FCM push
│   │   ├── TaskService/        # User tasks
│   │   └── RewardService/      # Points & rewards
│   ├── JobOrchestrator/        # Quartz + RabbitMQ consumers
│   └── Shared/                 # Shared libraries
└── docker-compose.yml          # Local development
```

## Quick Start

### Prerequisites
- .NET 9 SDK
- Flutter 3.x
- Docker & Docker Compose

### Development

```bash
# Start infrastructure
docker-compose up -d

# Run a service (example)
cd src/Services/AuthService/AuthService.Api
dotnet run

# Run mobile app
cd src/Apps/App.Client
flutter run

# Run admin dashboard
cd src/Apps/App.Admin
flutter run -d chrome
```

## Configuration

Environment variables pattern:
```
FIREBASE__PROJECT_ID=xxx
SHOPEE__APP_ID=xxx
GOOGLE__CLIENT_ID=xxx
JWT__SECRET=xxx
```

See `config/` for configuration templates.

## License

Proprietary - All rights reserved.
