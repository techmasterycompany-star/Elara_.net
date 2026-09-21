namespace Elara.Infrastructure.Options
{
    public sealed class EmailOptions
    {
        public string ClientUrl { get; set; } = null!;
        public SmtpOptions Smtp { get; set; } = new();
    }

    public sealed class SmtpOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? From { get; set; }
    }
}