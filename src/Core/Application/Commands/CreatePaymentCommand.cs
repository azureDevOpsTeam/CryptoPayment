using Application.DomainLayer.Entities;
using MediatR;

namespace Core.Application.Commands;

public class CreatePaymentCommand : IRequest<CreatePaymentResponse>
{
    public decimal Amount { get; set; }
    public NetworkType Network { get; set; }
    public CurrencyType Currency { get; set; }
}

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public NetworkType Network { get; set; }
    public DateTime ExpiresAt { get; set; }
}