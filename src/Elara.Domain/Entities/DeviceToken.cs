using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class DeviceToken
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public string Token { get; set; }
        public DevicePlatform Platform { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
