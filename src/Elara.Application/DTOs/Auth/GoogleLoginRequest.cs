using System.Text.Json.Serialization;

namespace Elara.Application.DTOs.Auth
{
    public class GoogleLoginRequest
    {
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = null!;
    }
}
