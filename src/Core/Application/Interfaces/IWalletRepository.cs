using Application.DomainLayer.Entities;

namespace Core.Application.Interfaces;

public interface IWalletRepository
{
    Task<Wallet> AddAsync(Wallet wallet);
    Task<Wallet?> GetByIdAsync(Guid id);
    Task<Wallet?> GetByAddressAsync(string address);
    Task<int> GetNextDerivationIndexAsync(NetworkType network);
    Task<IEnumerable<Wallet>> GetUnusedWalletsAsync(NetworkType network);
}