using Core.Application.Interfaces;
using Core.Application.Queries;
using MediatR;

namespace Core.Application.Handlers;

public class GetPaymentStatusHandler : IRequestHandler<GetPaymentStatusQuery, GetPaymentStatusResponse>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentStatusHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<GetPaymentStatusResponse> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithWalletAsync(request.PaymentId);
        
        if (payment == null)
            throw new InvalidOperationException("Payment not found");

        var timeRemaining = payment.ExpiresAt > DateTime.UtcNow 
            ? payment.ExpiresAt - DateTime.UtcNow 
            : TimeSpan.Zero;

        return new GetPaymentStatusResponse
        {
            PaymentId = payment.Id,
            WalletAddress = payment.Wallet.Address,
            Amount = payment.Amount,
            ReceivedAmount = payment.ReceivedAmount,
            Network = payment.Network,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            ExpiresAt = payment.ExpiresAt,
            CompletedAt = payment.CompletedAt,
            TransactionHash = payment.TransactionHash,
            TimeRemaining = timeRemaining
        };
    }
}