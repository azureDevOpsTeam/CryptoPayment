using Application.DomainLayer.Entities;
using Core.ApplicationLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation.PresentationApp.Models;

namespace Presentation.PresentationApp.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new CreatePaymentViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(CreatePaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _paymentService.CreatePaymentAsync(model.Amount, model.Network);
            
            return RedirectToAction("Status", new { paymentId = result.PaymentId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"خطا در ایجاد پرداخت: {ex.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Status(Guid paymentId)
    {
        try
        {
            var result = await _paymentService.GetPaymentStatusAsync(paymentId);
            
            var model = new PaymentStatusViewModel
            {
                PaymentId = result.PaymentId,
                WalletAddress = result.WalletAddress,
                Amount = result.Amount,
                ReceivedAmount = result.ReceivedAmount,
                Network = result.Network,
                Status = result.Status,
                CreatedAt = result.CreatedAt,
                ExpiresAt = result.ExpiresAt,
                CompletedAt = result.CompletedAt,
                TransactionHash = result.TransactionHash,
                TimeRemaining = result.TimeRemaining
            };

            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"خطا در دریافت وضعیت پرداخت: {ex.Message}";
            return View(new PaymentStatusViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckStatus(Guid paymentId)
    {
        try
        {
            var result = await _paymentService.GetPaymentStatusAsync(paymentId);
            return Json(new
            {
                success = true,
                status = result.Status.ToString(),
                receivedAmount = result.ReceivedAmount,
                transactionHash = result.TransactionHash,
                timeRemaining = result.TimeRemaining.TotalSeconds,
                completedAt = result.CompletedAt
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }
}