using Application.DomainLayer.Entities;

namespace Core.Application.Interfaces;

public interface IWalletService
{
    Task<Wallet> GenerateWalletAsync(NetworkType network);
    Task<decimal> GetBalanceAsync(string address, NetworkType network, CurrencyType currency);
    Task<bool> CheckTransactionAsync(string address, decimal expectedAmount, NetworkType network, CurrencyType currency);
    Task<string?> GetTransactionHashAsync(string address, decimal amount, NetworkType network, CurrencyType currency);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}