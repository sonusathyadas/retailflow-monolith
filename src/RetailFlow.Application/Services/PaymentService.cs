using System;
using System.Collections.Generic;
using RetailFlow.Application.DTOs;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Enums;
using RetailFlow.Domain.Events;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Messaging.Publishers;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IEventPublisher _publisher;
        private static readonly ILogger _log = Log.ForContext<PaymentService>();
        private static readonly Random _rng = new Random();

        public PaymentService(IPaymentRepository paymentRepo, IOrderRepository orderRepo,
            IEventPublisher publisher)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _publisher = publisher;
        }

        public PaymentDto ProcessPayment(ProcessPaymentRequest request)
        {
            var order = _orderRepo.GetById(request.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"Order {request.OrderId} not found.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Order is in status {order.Status} and cannot be paid.");

            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Initiated,
                TransactionReference = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow
            };

            // Update order status to processing
            order.Status = OrderStatus.PaymentProcessing;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepo.Update(order);

            _paymentRepo.Add(payment);
            _paymentRepo.SaveChanges();

            // Mock payment gateway — 90% success rate
            bool success = _rng.NextDouble() > 0.1;

            if (success)
            {
                payment.Status = PaymentStatus.Success;
                order.Status = OrderStatus.Paid;
                _log.Information("Payment {PaymentId} succeeded for order {OrderId}", payment.Id, order.Id);

                _publisher.Publish(new PaymentCompletedEvent
                {
                    PaymentId = payment.Id.ToString(),
                    OrderId = order.Id.ToString(),
                    Amount = payment.Amount
                });
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                order.Status = OrderStatus.Pending;
                _log.Warning("Payment {PaymentId} failed for order {OrderId}", payment.Id, order.Id);

                _publisher.Publish(new PaymentFailedEvent
                {
                    PaymentId = payment.Id.ToString(),
                    OrderId = order.Id.ToString(),
                    Reason = "Mock gateway declined"
                });
            }

            payment.UpdatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            _paymentRepo.Update(payment);
            _orderRepo.Update(order);
            _paymentRepo.SaveChanges();

            return MapToDto(payment);
        }

        public PaymentDto GetById(int paymentId)
        {
            var payment = _paymentRepo.GetById(paymentId);
            return payment == null ? null : MapToDto(payment);
        }

        public PaymentDto RetryPayment(int paymentId)
        {
            var payment = _paymentRepo.GetById(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment {paymentId} not found.");

            if (payment.Status != PaymentStatus.Failed)
                throw new InvalidOperationException("Only failed payments can be retried.");

            if (payment.RetryCount >= 3)
                throw new InvalidOperationException("Maximum retry attempts reached.");

            payment.RetryCount++;
            payment.Status = PaymentStatus.Initiated;
            payment.UpdatedAt = DateTime.UtcNow;
            _paymentRepo.Update(payment);
            _paymentRepo.SaveChanges();

            // Re-process
            return ProcessPayment(new ProcessPaymentRequest { OrderId = payment.OrderId });
        }

        private static PaymentDto MapToDto(Payment p)
        {
            return new PaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Status = p.Status.ToString(),
                Amount = p.Amount,
                TransactionReference = p.TransactionReference,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
