using System.ComponentModel.DataAnnotations;

namespace Application.DomainLayer.Entities;

public class Wallet
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string PrivateKey { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string PublicKey { get; set; } = string.Empty;
    
    [Required]
    public NetworkType Network { get; set; }
    
    public int DerivationIndex { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsUsed { get; set; }
    
    // Navigation properties
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum NetworkType
{
    BEP20 = 1,
    TRC20 = 2
}