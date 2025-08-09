using Application.DomainLayer.Entities;
using Core.Application.Interfaces;
using Infrastructure.InfrastructureLayer.Configuration;
using Microsoft.Extensions.Options;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Util;
using System.Numerics;
using System.Text;

namespace Infrastructure.InfrastructureLayer.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly string _masterSeed;

    public WalletService(IWalletRepository walletRepository, IBlockchainService blockchainService, IOptions<WalletSettings> walletSettings)
    {
        _walletRepository = walletRepository;
        _blockchainService = blockchainService;
        _masterSeed = walletSettings.Value.MasterSeed;
        
        if (string.IsNullOrEmpty(_masterSeed))
        {
            throw new InvalidOperationException("Master seed is not configured. Please set WalletSettings:MasterSeed in configuration.");
        }
    }

    public async Task<Application.DomainLayer.Entities.Wallet> GenerateWalletAsync(NetworkType network)
    {

        var derivationIndex = await _walletRepository.GetNextDerivationIndexAsync(network);
        
        string address, privateKey, publicKey;
        
        if (network == NetworkType.BEP20)
        {
            // Generate BEP20 (Binance Smart Chain) wallet using proper derivation path
            var mnemonic = new Mnemonic(_masterSeed);
            var masterKey = mnemonic.DeriveExtKey();
            // Use BIP44 derivation path for Ethereum/BSC: m/44'/60'/0'/0/index
            var derivedKey = masterKey.Derive(new KeyPath($"m/44'/60'/0'/0/{derivationIndex}"));
            
            privateKey = derivedKey.PrivateKey.ToHex();
            publicKey = derivedKey.PrivateKey.PubKey.ToHex();
            
            // Convert to Ethereum/BSC address format using Keccak256
            var pubKeyBytes = derivedKey.PrivateKey.PubKey.ToBytes();
            var keccak = new Sha3Keccack();
            var hash = keccak.CalculateHash(pubKeyBytes.Skip(1).ToArray());
            var addressBytes = hash.Skip(12).ToArray();
            address = "0x" + Convert.ToHexString(addressBytes).ToLower();
        }
        else // TRC20
        {
            // Generate TRC20 (TRON) wallet
            var mnemonic = new Mnemonic(_masterSeed);
            var masterKey = mnemonic.DeriveExtKey();
            var derivedKey = masterKey.Derive(new KeyPath($"m/44'/195'/0'/0/{derivationIndex}"));
            
            privateKey = derivedKey.PrivateKey.ToHex();
            publicKey = derivedKey.PrivateKey.PubKey.ToHex();
            
            // Convert to TRON address format
            address = ConvertToTronAddress(derivedKey.PrivateKey.PubKey.ToBytes());
        }

        var walletEntity = new Application.DomainLayer.Entities.Wallet
        {
            Id = Guid.NewGuid(),
            Address = address,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Network = network,
            DerivationIndex = derivationIndex,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _walletRepository.AddAsync(walletEntity);
        return walletEntity;
    }

    public async Task<decimal> GetBalanceAsync(string address, NetworkType network, CurrencyType currency)
    {
        if (currency == CurrencyType.USDT)
        {
            return await _blockchainService.GetUSDTBalanceAsync(address, network);
        }
        else // BNB
        {
            return await _blockchainService.GetBNBBalanceAsync(address, network);
        }
    }

    public async Task<bool> CheckTransactionAsync(string address, decimal expectedAmount, NetworkType network, CurrencyType currency)
    {
        var balance = await GetBalanceAsync(address, network, currency);
        return balance >= expectedAmount;
    }

    public async Task<string?> GetTransactionHashAsync(string address, decimal amount, NetworkType network, CurrencyType currency)
    {
        return await _blockchainService.GetTransactionHashAsync(address, amount, network, currency);
    }

    private string ConvertToTronAddress(byte[] publicKeyBytes)
    {
        // Simplified TRON address generation
        // In production, use proper TRON SDK
        var hash = System.Security.Cryptography.SHA256.HashData(publicKeyBytes);
        var addressBytes = new byte[21];
        addressBytes[0] = 0x41; // TRON mainnet prefix
        Array.Copy(hash, 12, addressBytes, 1, 20);
        
        return Base58CheckEncoding.Encode(addressBytes);
    }
}

public static class Base58CheckEncoding
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    
    public static string Encode(byte[] data)
    {
        // Add checksum
        var hash1 = System.Security.Cryptography.SHA256.HashData(data);
        var hash2 = System.Security.Cryptography.SHA256.HashData(hash1);
        var dataWithChecksum = data.Concat(hash2.Take(4)).ToArray();
        
        // Convert to Base58
        var result = "";
        var value = new System.Numerics.BigInteger(dataWithChecksum.Reverse().Concat(new byte[] { 0 }).ToArray());
        
        while (value > 0)
        {
            var remainder = (int)(value % 58);
            result = Alphabet[remainder] + result;
            value /= 58;
        }
        
        // Add leading zeros
        foreach (var b in dataWithChecksum)
        {
            if (b == 0)
                result = "1" + result;
            else
                break;
        }
        
        return result;
    }
}