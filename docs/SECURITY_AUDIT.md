# Security Audit Checklist

## Authentication & Authorization

### JWT Security
- [x] JWT tokens have short expiration (15 minutes)
- [x] Refresh tokens stored securely
- [x] Token revocation mechanism implemented
- [ ] Rotate signing keys periodically
- [ ] Implement token blacklisting for immediate revocation

### OAuth2
- [x] Google OAuth2 for mobile app
- [x] Credentials authentication for admin
- [ ] Validate OAuth token signatures
- [ ] Check token audience and issuer

## API Security

### Input Validation
- [ ] Validate all user inputs (FluentValidation)
- [ ] Sanitize inputs to prevent injection attacks
- [ ] Implement request size limits
- [ ] Rate limiting per user/IP

### Headers
- [ ] Add security headers (HSTS, X-Frame-Options, etc.)
- [ ] CORS properly configured
- [ ] Remove sensitive headers (Server, X-Powered-By)

## Data Protection

### Encryption
- [ ] HTTPS enforced everywhere
- [ ] Sensitive data encrypted at rest
- [ ] Database connections use TLS

### PII Handling
- [ ] User emails hashed/encrypted
- [ ] Logs don't contain sensitive data
- [ ] Implement data retention policies

## Infrastructure

### MongoDB
- [ ] Enable authentication
- [ ] Use strong passwords
- [ ] Restrict network access
- [ ] Enable audit logging

### Redis
- [ ] Enable password authentication
- [ ] Use TLS for connections
- [ ] Disable dangerous commands (FLUSHALL)

### RabbitMQ
- [ ] Enable authentication
- [ ] Use TLS for connections
- [ ] Limit permissions per service

## Code Security

### Dependencies
- [ ] Run `npm audit` / `dotnet list package --vulnerable`
- [ ] Update vulnerable packages
- [ ] Pin dependency versions

### Secrets Management
- [x] Secrets in environment variables
- [ ] Use secret manager in production (Azure Key Vault, etc.)
- [ ] Rotate secrets regularly

## Testing

### Security Tests
- [ ] Run OWASP ZAP scan
- [ ] SQL/NoSQL injection tests
- [ ] XSS vulnerability tests
- [ ] CSRF protection tests

## Monitoring

### Logging
- [ ] Log authentication attempts
- [ ] Log suspicious activities
- [ ] Implement alerting for anomalies

## Action Items

| Priority | Item | Status |
|----------|------|--------|
| High | Add rate limiting to API Gateway | TODO |
| High | Add input validation with FluentValidation | TODO |
| Medium | Set up security headers middleware | TODO |
| Medium | MongoDB authentication | TODO |
| Low | Secret rotation automation | TODO |
