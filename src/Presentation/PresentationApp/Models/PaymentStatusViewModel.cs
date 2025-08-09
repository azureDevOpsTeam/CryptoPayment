using Application.DomainLayer.Entities;

namespace Presentation.PresentationApp.Models;

public class PaymentStatusViewModel
{
    public Guid PaymentId { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ReceivedAmount { get; set; }
    public NetworkType Network { get; set; }
    public CurrencyType Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? TransactionHash { get; set; }
    public TimeSpan TimeRemaining { get; set; }

    public string NetworkDisplayName => Network switch
    {
        NetworkType.BEP20 => "BEP20 (Binance Smart Chain)",
        NetworkType.TRC20 => "TRC20 (TRON)",
        _ => Network.ToString()
    };

    public string StatusDisplayName => Status switch
    {
        PaymentStatus.Pending => "در انتظار پرداخت",
        PaymentStatus.Completed => "پرداخت موفق",
        PaymentStatus.Expired => "منقضی شده",
        PaymentStatus.Failed => "ناموفق",
        _ => Status.ToString()
    };

    public string StatusCssClass => Status switch
    {
        PaymentStatus.Pending => "warning",
        PaymentStatus.Completed => "success",
        PaymentStatus.Expired => "danger",
        PaymentStatus.Failed => "danger",
        _ => "secondary"
    };

    public bool IsExpired => TimeRemaining <= TimeSpan.Zero && Status == PaymentStatus.Pending;
    public bool IsCompleted => Status == PaymentStatus.Completed;
    public bool IsPending => Status == PaymentStatus.Pending && !IsExpired;
}