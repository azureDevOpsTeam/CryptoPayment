using Application.DomainLayer.Entities;

namespace Core.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByIdWithWalletAsync(Guid id);
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    Task<IEnumerable<Payment>> GetExpiredPaymentsAsync();
    Task UpdateAsync(Payment payment);
}