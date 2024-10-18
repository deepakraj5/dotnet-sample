using Microsoft.AspNetCore.Authorization;
using SampleDotNet.Data;

namespace SampleDotNet.Services
{
    public class CustomAuthorizationHandler: AuthorizationHandler<ValidateAccessToken>
    {
        private readonly ApplicationDBContext _dbContext;

        public CustomAuthorizationHandler(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidateAccessToken requirement)
        {
            try
            {
                var jtiClaim = context.User.Claims.SingleOrDefault(claim => claim.Type == "jti");

                if (jtiClaim == null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                var userToken = _dbContext.UserAccTokens.SingleOrDefault(token => token.Token == jtiClaim.Value.ToString());
                // implement to check IsBlackListed
                if (userToken is null)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
                
                context.Succeed(requirement);
                return Task.CompletedTask;
            } catch
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
    }
}
