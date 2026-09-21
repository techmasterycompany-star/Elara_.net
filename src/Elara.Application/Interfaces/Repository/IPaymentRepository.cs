using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(long id);
        Task<Payment?> GetByOrderIdAsync(long orderId);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
        Task<IEnumerable<Payment>> GetByUserIdAsync(long userId);
        Task<IEnumerable<Payment>> GetByStatusAsync(Elara.Domain.Enums.PaymentStatus status);
        Task<Payment?> GetLatestFailedPaymentByOrderIdAsync(long orderId);
    }
}