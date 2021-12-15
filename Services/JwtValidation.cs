using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SecureIsolatedAzFunction
{
    public class JwtValidation
    {
        private IConfiguration _configuration;
        private const string scopeType = @"http://schemas.microsoft.com/identity/claims/scope";
        private static ConfigurationManager<OpenIdConnectConfiguration>? _configurationManager;
        private string _wellKnownEndpoint = string.Empty;
        private string _tenantId = string.Empty;
        private string _audience = string.Empty;
        private string _instance = string.Empty;

        public JwtValidation(IConfiguration configuration)
        {
            _configuration = configuration;
            _tenantId = _configuration["AzureAd:TenantId"];
            _audience = $"api://{_configuration["AzureAd:ClientId"]}";
            _instance = _configuration["AzureAd:Instance"];
            _wellKnownEndpoint = $"{_instance}{_tenantId}/.well-known/openid-configuration";
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string token, ILogger logger)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var oidcWellknownEndpoints = await GetOidcWellKnownConfiguration(logger);
            var tokenValidator = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = _audience,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                ValidIssuer = oidcWellknownEndpoints.Issuer
            };

            try
            {
                SecurityToken securityToken;
                var claimsPrincipal = tokenValidator.ValidateToken(token, validationParameters, out securityToken);
                
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }

            return null;
        }

        private async Task<OpenIdConnectConfiguration> GetOidcWellKnownConfiguration(ILogger logger)
        {
            if(_configurationManager == null)
            {
                logger.LogDebug($"Getting Azure AD OIDC well known endpoint {_wellKnownEndpoint}");
                _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(_wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());
            }

            return await _configurationManager.GetConfigurationAsync();
        }
    }
}