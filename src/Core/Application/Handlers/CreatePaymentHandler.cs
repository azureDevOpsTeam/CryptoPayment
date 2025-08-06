using Application.DomainLayer.Entities;
using Core.Application.Commands;
using Core.Application.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IWalletService _walletService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentHandler(
        IWalletRepository walletRepository,
        IPaymentRepository paymentRepository,
        IWalletService walletService,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _paymentRepository = paymentRepository;
        _walletService = walletService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        // Generate new HD wallet for the payment
        var wallet = await _walletService.GenerateWalletAsync(request.Network);
        
        // Save wallet to database
        await _walletRepository.AddAsync(wallet);
        
        // Create payment record
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Network = request.Network,
            WalletId = wallet.Id,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(20)
        };
        
        await _paymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new CreatePaymentResponse
        {
            PaymentId = payment.Id,
            WalletAddress = wallet.Address,
            Amount = payment.Amount,
            Network = payment.Network,
            ExpiresAt = payment.ExpiresAt
        };
    }
}