using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace UOStudio.Common.Core.Extensions
{
    public static class IPrincipalExtensions
    {
        public static int GetUserId(this IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claimsPrincipal)
            {
                var nameIdentifier = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                if (nameIdentifier == null)
                {
                    throw new InvalidOperationException("Principal contains no nameIdentifier claim");
                }

                if (int.TryParse(nameIdentifier.Value, out var userId))
                {
                    return userId;
                }

                throw new InvalidOperationException("NameIdentifier is not of type int");
            }

            throw new InvalidOperationException("Principal is not a claims principal");
        }
    }
}
