using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface IPaymentService
    {
        PaymentDto ProcessPayment(ProcessPaymentRequest request);
        PaymentDto GetById(int paymentId);
        PaymentDto RetryPayment(int paymentId);
    }
}
