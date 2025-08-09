using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DomainLayer.Entities;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,8)")]
    public decimal Amount { get; set; }
    
    [Required]
    public NetworkType Network { get; set; }
    
    [Required]
    public CurrencyType Currency { get; set; }
    
    [Required]
    public Guid WalletId { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; }
    
    [MaxLength(100)]
    public string? TransactionHash { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? ReceivedAmount { get; set; }
    
    // Navigation properties
    [ForeignKey("WalletId")]
    public virtual Wallet Wallet { get; set; } = null!;
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Expired = 3,
    Failed = 4
}