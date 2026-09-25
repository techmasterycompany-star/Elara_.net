namespace Elara.Application.DTOs
{
    public class RedeemPointsResultDto
        {
            public bool IsSuccess { get; set; }
            public string? ErrorMessage { get; set; }
            public int RemainingBalance { get; set; }
        }
    
}