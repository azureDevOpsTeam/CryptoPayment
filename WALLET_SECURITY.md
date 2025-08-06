# Wallet Security Configuration

## Master Seed Configuration

The application now uses secure configuration for the HD Wallet master seed instead of hard-coding it in the source code.

### Configuration Methods

#### 1. appsettings.json (Development)
```json
{
  "WalletSettings": {
    "MasterSeed": "your-secure-seed-phrase-here"
  }
}
```

#### 2. Environment Variables (Production)
```bash
# Windows
set WalletSettings__MasterSeed=your-secure-seed-phrase-here

# Linux/Mac
export WalletSettings__MasterSeed="your-secure-seed-phrase-here"
```

#### 3. User Secrets (Development)
```bash
dotnet user-secrets set "WalletSettings:MasterSeed" "your-secure-seed-phrase-here" --project src/Presentation/PresentationApp
```

#### 4. Azure Key Vault (Production)
For production environments, it's recommended to use Azure Key Vault or similar secure storage services.

### Security Best Practices

1. **Never commit seeds to source control**
2. **Use different seeds for different environments**
3. **Store production seeds in secure vaults**
4. **Regularly rotate seeds if possible**
5. **Use strong, randomly generated seed phrases**

### Current Implementation

- The `WalletService` now accepts `IOptions<WalletSettings>` in its constructor
- Configuration is validated at startup
- If no seed is provided, the application will throw an exception
- Supports all standard .NET configuration providers

### Migration from Hard-coded Seed

The previous hard-coded seed has been removed and replaced with configuration-based approach for better security and flexibility.