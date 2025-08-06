using Application.DomainLayer.Entities;
using MediatR;

namespace Core.Application.Queries;

public class GetPaymentStatusQuery : IRequest<GetPaymentStatusResponse>
{
    public Guid PaymentId { get; set; }
}

public class GetPaymentStatusResponse
{
    public Guid PaymentId { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public NetworkType Network { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TransactionHash { get; set; }
    public TimeSpan TimeRemaining { get; set; }
}