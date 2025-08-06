using Application.DomainLayer.Entities;
using Core.Application.Interfaces;
using Infrastructure.InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.InfrastructureLayer.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> AddAsync(Wallet wallet)
    {
        _context.Wallets.Add(wallet);
        return wallet;
    }

    public async Task<Wallet?> GetByIdAsync(Guid id)
    {
        return await _context.Wallets.FindAsync(id);
    }

    public async Task<Wallet?> GetByAddressAsync(string address)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.Address == address);
    }

    public async Task<int> GetNextDerivationIndexAsync(NetworkType network)
    {
        var lastIndex = await _context.Wallets
            .Where(w => w.Network == network)
            .MaxAsync(w => (int?)w.DerivationIndex) ?? -1;
            
        return lastIndex + 1;
    }

    public async Task<IEnumerable<Wallet>> GetUnusedWalletsAsync(NetworkType network)
    {
        return await _context.Wallets
            .Where(w => w.Network == network && !w.IsUsed)
            .ToListAsync();
    }
}