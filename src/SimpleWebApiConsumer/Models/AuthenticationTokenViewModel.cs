using System.Text.Json.Serialization;

namespace SimpleWebApiConsumer.Models
{
    public class AuthenticationTokenViewModel
    {

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

    }
}
