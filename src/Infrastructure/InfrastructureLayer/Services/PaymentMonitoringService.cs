using Application.DomainLayer.Entities;
using Core.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.InfrastructureLayer.Services;

public class PaymentMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentMonitoringService> _logger;

    public PaymentMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<PaymentMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Monitoring Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorPaymentsAsync();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Check every 30 seconds
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while monitoring payments.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute on error
            }
        }

        _logger.LogInformation("Payment Monitoring Service stopped.");
    }

    private async Task MonitorPaymentsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Get all pending payments
        var pendingPayments = await paymentRepository.GetPendingPaymentsAsync();

        foreach (var payment in pendingPayments)
        {
            try
            {
                // Check if payment has expired
                if (payment.ExpiresAt <= DateTime.UtcNow)
                {
                    payment.Status = PaymentStatus.Expired;
                    await paymentRepository.UpdateAsync(payment);
                    _logger.LogInformation($"Payment {payment.Id} marked as expired.");
                    continue;
                }

                // Check blockchain for transaction
                var hasTransaction = await walletService.CheckTransactionAsync(
                    payment.Wallet.Address, 
                    payment.Amount, 
                    payment.Network);

                if (hasTransaction)
                {
                    // Get transaction details
                    var transactionHash = await walletService.GetTransactionHashAsync(
                        payment.Wallet.Address, 
                        payment.Amount, 
                        payment.Network);

                    var receivedAmount = await walletService.GetBalanceAsync(
                        payment.Wallet.Address, 
                        payment.Network);

                    // Update payment status
                    payment.Status = PaymentStatus.Completed;
                    payment.CompletedAt = DateTime.UtcNow;
                    payment.TransactionHash = transactionHash;
                    payment.ReceivedAmount = receivedAmount;
                    payment.Wallet.IsUsed = true;

                    await paymentRepository.UpdateAsync(payment);
                    
                    _logger.LogInformation($"Payment {payment.Id} completed successfully. Transaction: {transactionHash}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error monitoring payment {payment.Id}");
            }
        }

        // Save all changes
        await unitOfWork.SaveChangesAsync();
    }
}