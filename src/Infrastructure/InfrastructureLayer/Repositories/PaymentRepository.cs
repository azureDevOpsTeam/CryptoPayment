using Application.DomainLayer.Entities;
using Core.Application.Interfaces;
using Infrastructure.InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.InfrastructureLayer.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> AddAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        return payment;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task<Payment?> GetByIdWithWalletAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.Wallet)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.Wallet)
            .Where(p => p.Status == PaymentStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetExpiredPaymentsAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == PaymentStatus.Pending && p.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}