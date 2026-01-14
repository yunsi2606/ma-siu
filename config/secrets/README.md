# Secrets Directory

This directory contains sensitive configuration files that should NOT be committed to version control.

## Required Files

1. **firebase-credentials.json** - Firebase Admin SDK credentials
   - Download from: Firebase Console → Project Settings → Service Accounts → Generate New Private Key
   
2. **affiliate-keys.json** (optional) - Local affiliate API keys for development

## Security Notes

- This entire `secrets/` directory is gitignored (except this README)
- Never commit actual credentials
- Use environment variables in production
- Rotate credentials regularly
