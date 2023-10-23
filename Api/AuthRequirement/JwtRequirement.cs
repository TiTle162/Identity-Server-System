using Microsoft.AspNetCore.Authorization;

namespace Api.AuthRequirement
{
    public class JwtRequirement : IAuthorizationRequirement { }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _client;
        private readonly HttpContext _httpContext;

        public JwtRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) 
        {
            _client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        // Check Access Token
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            JwtRequirement requirement)
        {
            // If Access Token was sent.
            if(_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                // Get Access Token
                var accessToken = authHeader.ToString().Split(' ')[1];

                // Sent Access Token to check and get response.
                var response = await _client
                    .GetAsync($"https://localhost:44382/oauth/validate?access_token={accessToken}");

                // If Access Token that check is valid.
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
