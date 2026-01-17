# Load Testing Guide

## Prerequisites

Install k6:
```powershell
# Windows (winget)
winget install Grafana.k6

# Or download from https://dl.k6.io/msi/k6-latest-amd64.msi
```

## Run Load Tests

### Against Local Environment
```powershell
cd tests/load
k6 run api-load-test.js
```

### Against Staging
```powershell
k6 run -e BASE_URL=https://staging.masiu.vn api-load-test.js
```

## Test Scenarios

### Smoke Test (Quick validation)
```powershell
k6 run --vus 1 --duration 30s api-load-test.js
```

### Load Test (Normal load)
```powershell
k6 run api-load-test.js
```

### Stress Test (Find limits)
```powershell
k6 run --stage 1m:100,2m:200,2m:300,1m:0 api-load-test.js
```

### Spike Test (Sudden traffic surge)
```powershell
k6 run --stage 10s:10,10s:200,10s:10 api-load-test.js
```

## Interpreting Results

| Metric | Good | Warning | Critical |
|--------|------|---------|----------|
| p95 Response Time | <200ms | 200-500ms | >500ms |
| Error Rate | <1% | 1-5% | >5% |
| Throughput | >100 req/s | 50-100 | <50 |

## Export Results

```powershell
# JSON output
k6 run --out json=results.json api-load-test.js

# InfluxDB (for Grafana)
k6 run --out influxdb=http://localhost:8086/k6 api-load-test.js
```
