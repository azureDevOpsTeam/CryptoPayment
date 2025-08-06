using Application.DomainLayer.Entities;
using Core.Application.Commands;
using Core.Application.Queries;
using MediatR;

namespace Core.ApplicationLayer.Services;

public interface IPaymentService
{
    Task<CreatePaymentResponse> CreatePaymentAsync(decimal amount, NetworkType network);
    Task<GetPaymentStatusResponse> GetPaymentStatusAsync(Guid paymentId);
}

public class PaymentService : IPaymentService
{
    private readonly IMediator _mediator;

    public PaymentService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CreatePaymentResponse> CreatePaymentAsync(decimal amount, NetworkType network)
    {
        var command = new CreatePaymentCommand
        {
            Amount = amount,
            Network = network
        };

        return await _mediator.Send(command);
    }

    public async Task<GetPaymentStatusResponse> GetPaymentStatusAsync(Guid paymentId)
    {
        var query = new GetPaymentStatusQuery
        {
            PaymentId = paymentId
        };

        return await _mediator.Send(query);
    }
}