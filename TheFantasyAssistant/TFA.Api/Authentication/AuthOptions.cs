using System.ComponentModel.DataAnnotations;
using TFA.Application.Config;

namespace TFA.Api.Authentication
{
    public class AuthOptions : IConfigurationOptions
    {
        public const string ApiKeyHeaderName = "x-api-key";
        public const string ClientKeyHeaderName = "x-client-connection-id";

        public string Key => "Authentication";

        [Required]
        public string ApiKey { get; init; } = string.Empty;

        [Required]
        public string ClientUrl { get; init; } = string.Empty;
    }
}
