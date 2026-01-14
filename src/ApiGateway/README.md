# API Gateway

## Overview

YARP-based reverse proxy gateway cho Mã Siu platform. Gateway chịu trách nhiệm:

- **Routing**: Điều phối requests đến các microservices
- **Authentication**: Validate JWT tokens
- **Rate Limiting**: Giới hạn request rate cho public endpoints
- **Affiliate Redirect**: Endpoint `/go/{postId}` để redirect affiliate links

## Ports Configuration

| Service | Port |
|---------|------|
| ApiGateway | 5100 |
| AuthService | 5101 |
| UserService | 5102 |
| PostService | 5103 |
| VoucherService | 5104 |
| AffiliateService | 5105 |
| NotificationService | 5106 |
| TaskService | 5107 |
| RewardService | 5108 |

## Routes

| Path | Service | Auth Required |
|------|---------|---------------|
| `/api/auth/*` | AuthService | ❌ |
| `/api/users/*` | UserService | ✅ |
| `/api/posts/*` | PostService | ✅ |
| `/api/vouchers/*` | VoucherService | ✅ |
| `/api/affiliates/*` | AffiliateService | ✅ |
| `/api/notifications/*` | NotificationService | ✅ |
| `/api/tasks/*` | TaskService | ✅ |
| `/api/rewards/*` | RewardService | ✅ |
| `/go/{postId}` | AffiliateService (redirect) | ❌ |
| `/health` | Gateway health check | ❌ |

## Rate Limiting

> ⚠️ Rate limiting chỉ áp dụng cho **public HTTP endpoints**, KHÔNG áp dụng cho internal gRPC calls.

| Endpoint | Limit |
|----------|-------|
| General | 100 req/min |
| `/api/auth/*` | 10 req/min (prevent brute force) |

## JWT Configuration

```json
{
  "Jwt": {
    "Secret": "YOUR_SECRET_KEY",
    "Issuer": "gateway-sale.nhatcuong.io.vn",
    "Audience": "masiu-app"
  }
}
```

Token validation:
- Validate Issuer ✅
- Validate Audience ✅  
- Validate Lifetime ✅
- Validate Signing Key ✅
- ClockSkew = 0 (strict expiry)

## CORS

Cho phép requests từ:
- `https://admin-sale.nhatcuong.io.vn` (Production)
- `http://localhost:3000` (Development)

## Running

```bash
cd src/ApiGateway
dotnet run
# Gateway starts at http://localhost:5100
```
