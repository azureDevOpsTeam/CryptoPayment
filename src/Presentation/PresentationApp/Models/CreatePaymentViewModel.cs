using Application.DomainLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Presentation.PresentationApp.Models;

public class CreatePaymentViewModel
{
    [Required(ErrorMessage = "مقدار USDT الزامی است")]
    [Range(0.01, 1000000, ErrorMessage = "مقدار باید بین 0.01 تا 1,000,000 USDT باشد")]
    [Display(Name = "مقدار USDT")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "انتخاب شبکه الزامی است")]
    [Display(Name = "شبکه")]
    public NetworkType Network { get; set; }

    public List<NetworkOption> NetworkOptions { get; set; } = new()
    {
        new NetworkOption { Value = NetworkType.BEP20, Text = "BEP20 (Binance Smart Chain)" },
        new NetworkOption { Value = NetworkType.TRC20, Text = "TRC20 (TRON)" }
    };
}

public class NetworkOption
{
    public NetworkType Value { get; set; }
    public string Text { get; set; } = string.Empty;
}