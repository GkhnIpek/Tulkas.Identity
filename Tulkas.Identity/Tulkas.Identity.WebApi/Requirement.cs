using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tulkas.Identity.WebApi
{
    public class ExpireDateExchangeRequirement : IAuthorizationRequirement
    {

    }

    public class ExpireDateExchangeHandler : AuthorizationHandler<ExpireDateExchangeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequirement requirement)
        {
            if (context.User?.Identity != null)
            {
                var claim = context.User.Claims
                    .FirstOrDefault(x => x.Type == "ExpireDateExchange" && x.Value != null);

                if (claim != null)
                {
                    if (DateTime.Now < Convert.ToDateTime(claim.Value))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
