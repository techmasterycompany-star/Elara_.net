namespace Elara.Application.DTOs
{
    public class RegisterDeviceResultDto
        {
            public bool IsSuccess { get; set; }
            public string? ErrorCode { get; set; }
            public string? ErrorMessage { get; set; }
        }
    
}