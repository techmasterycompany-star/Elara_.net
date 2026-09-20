namespace Elara.Application.DTOs
{
    public class LoyaltyTransactionDto
        {
            public int Points { get; set; }
            public string Type { get; set; } = string.Empty;
            public long? OrderId { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    
}