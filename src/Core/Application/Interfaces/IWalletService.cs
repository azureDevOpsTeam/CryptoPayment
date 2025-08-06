using Application.DomainLayer.Entities;

namespace Core.Application.Interfaces;

public interface IWalletService
{
    Task<Wallet> GenerateWalletAsync(NetworkType network);
    Task<decimal> GetBalanceAsync(string address, NetworkType network);
    Task<bool> CheckTransactionAsync(string address, decimal expectedAmount, NetworkType network);
    Task<string?> GetTransactionHashAsync(string address, decimal amount, NetworkType network);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}