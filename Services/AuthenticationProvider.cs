using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SecureIsolatedAzFunction
{
    public class AuthenticationProvider
    {
        private readonly JwtValidation _jwtValidation;

        public AuthenticationProvider(JwtValidation jwtValidation)
        {
            _jwtValidation = jwtValidation;
        }
        public async Task<ClaimsPrincipal> AuthenticateAsync(FunctionContext context, HttpRequestData req)
        {
            var logger = context.GetLogger("AuthenticationProvider");
            return await _jwtValidation.ValidateTokenAsync(getAccessTokenFromHeaders(req), logger);
        }

        public async Task<ClaimsPrincipal> AuthenticateAsync(FunctionContext context, string token)
        {
            var logger = context.GetLogger("AuthenticationProvider");
            return await _jwtValidation.ValidateTokenAsync(token, logger);
        }

        private string getAccessTokenFromHeaders(HttpRequestData req)
        {
            var token = req.Headers.Where(x => x.Key == "Authorization")
                                   .First().Value.First()
                                   .Substring("Bearer ".Length);
            
            return token;
        }
    }
}