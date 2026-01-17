# Cloudflare Tunnel Setup Guide

## Prerequisites
1. Cloudflare account with a domain
2. `cloudflared` installed on your server

## Installation

### Install cloudflared (Linux)
```bash
curl -L --output cloudflared.deb https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared.deb
```

### Windows
```powershell
winget install Cloudflare.cloudflared
```

## Setup

### 1. Authenticate with Cloudflare
```bash
cloudflared tunnel login
```

### 2. Create a tunnel
```bash
cloudflared tunnel create masiu-tunnel
```

This creates a credentials file at `~/.cloudflared/<TUNNEL_ID>.json`

### 3. Configure DNS
```bash
cloudflared tunnel route dns masiu-tunnel api.masiu.vn
cloudflared tunnel route dns masiu-tunnel admin.masiu.vn
```

### 4. Copy configuration
```bash
sudo mkdir -p /etc/cloudflared
sudo cp deploy/cloudflared/config.yml /etc/cloudflared/
sudo cp ~/.cloudflared/<TUNNEL_ID>.json /etc/cloudflared/credentials.json
```

### 5. Run as service
```bash
sudo cloudflared service install
sudo systemctl start cloudflared
sudo systemctl enable cloudflared
```

## Docker Compose Integration
Add to docker-compose.yml:
```yaml
cloudflared:
  image: cloudflare/cloudflared:latest
  command: tunnel --no-autoupdate run
  volumes:
    - ./deploy/cloudflared/config.yml:/etc/cloudflared/config.yml:ro
    - ./credentials.json:/etc/cloudflared/credentials.json:ro
  depends_on:
    - nginx
```

## Verify
```bash
cloudflared tunnel info masiu-tunnel
curl https://api.masiu.vn/health
```
