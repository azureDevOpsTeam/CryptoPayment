# Crypto Payment Gateway - درگاه پرداخت کریپتو

A secure cryptocurrency payment gateway built with .NET 9, supporting USDT payments on BEP20 (Binance Smart Chain) and TRC20 (TRON) networks.

## Features

- **Clean Architecture**: Implements Clean Architecture principles with clear separation of concerns
- **CQRS Pattern**: Uses Command Query Responsibility Segregation with MediatR
- **ServiceBase Pattern**: Services are separated from CQRS handlers
- **HD Wallet Generation**: Generates hierarchical deterministic wallets for each payment
- **Multi-Network Support**: Supports both BEP20 and TRC20 networks
- **Real-time Monitoring**: Background service monitors blockchain transactions
- **Responsive UI**: Modern, RTL-supported web interface
- **Timer-based Payments**: 20-minute payment window with countdown timer

## Architecture

```
src/
├── Core/
│   ├── Application/           # CQRS Commands, Queries, and Handlers
│   └── ApplicationLayer/      # Application Services (ServiceBase pattern)
├── Application/
│   └── DomainLayer/          # Domain Entities and Business Logic
├── Infrastructure/
│   └── InfrastructureLayer/  # Data Access, External Services
└── Presentation/
    └── PresentationApp/      # Web UI (ASP.NET Core MVC)
```

## Technologies Used

- **.NET 9**: Latest .NET framework
- **Entity Framework Core**: Database ORM
- **SQL Server**: Database
- **MediatR**: CQRS implementation
- **NBitcoin**: Bitcoin/Crypto wallet generation
- **Nethereum**: Ethereum/BEP20 blockchain interaction
- **Bootstrap 5**: UI framework with RTL support
- **ASP.NET Core MVC**: Web framework

## Setup Instructions

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CryptoPayment
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string**
   
   Edit `src/Presentation/PresentationApp/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CryptoPaymentDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run the application**
   ```bash
   cd src/Presentation/PresentationApp
   dotnet run
   ```

5. **Access the application**
   
   Open your browser and navigate to `https://localhost:5001`

## Usage

### Creating a Payment

1. **Enter Payment Details**
   - Enter the USDT amount (0.01 - 1,000,000)
   - Select network (BEP20 or TRC20)
   - Click "ایجاد آدرس پرداخت"

2. **Complete Payment**
   - Copy the generated wallet address
   - Send the exact USDT amount to the address
   - Wait for blockchain confirmation

3. **Payment Monitoring**
   - 20-minute countdown timer
   - Automatic transaction detection
   - Real-time status updates

## Database Schema

### Wallets Table
- `Id` (Guid): Primary key
- `Address` (string): Wallet address
- `PrivateKey` (string): Encrypted private key
- `PublicKey` (string): Public key
- `Network` (enum): BEP20 or TRC20
- `DerivationIndex` (int): HD wallet derivation index
- `CreatedAt` (DateTime): Creation timestamp
- `IsUsed` (bool): Usage flag

### Payments Table
- `Id` (Guid): Primary key
- `Amount` (decimal): Payment amount
- `Network` (enum): Network type
- `WalletId` (Guid): Associated wallet
- `Status` (enum): Payment status
- `TransactionHash` (string): Blockchain transaction hash
- `CreatedAt` (DateTime): Creation timestamp
- `ExpiresAt` (DateTime): Expiration timestamp
- `CompletedAt` (DateTime): Completion timestamp
- `ReceivedAmount` (decimal): Actual received amount

## Security Features

- **HD Wallet Generation**: Each payment gets a unique address
- **Private Key Storage**: Encrypted storage in database
- **Network Validation**: Ensures payments on correct networks
- **Amount Verification**: Validates exact payment amounts
- **Expiration Handling**: Automatic payment expiration

## Configuration

### Blockchain RPC Endpoints

Update the following in `BlockchainService.cs`:

```csharp
private readonly string _bscRpcUrl = "https://bsc-dataseed.binance.org/";
private readonly string _tronRpcUrl = "https://api.trongrid.io";
```

### Contract Addresses

```csharp
private readonly string _usdtBep20Contract = "0x55d398326f99059fF775485246999027B3197955";
private readonly string _usdtTrc20Contract = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t";
```

## Development

### Adding New Networks

1. Add network to `NetworkType` enum
2. Update `WalletService` for wallet generation
3. Update `BlockchainService` for transaction monitoring
4. Update UI components

### Extending Payment Methods

1. Create new entities in Domain Layer
2. Add CQRS commands/queries in Core.Application
3. Implement handlers
4. Update services in ApplicationLayer
5. Add UI components

## Production Considerations

- **Security**: Use proper key management (Azure Key Vault, etc.)
- **Monitoring**: Add comprehensive logging and monitoring
- **Scaling**: Consider database sharding for high volume
- **Backup**: Implement secure backup for private keys
- **Rate Limiting**: Add rate limiting for API endpoints
- **SSL/TLS**: Ensure HTTPS in production

## License

This project is licensed under the MIT License.

## Support

For support and questions, please create an issue in the repository.