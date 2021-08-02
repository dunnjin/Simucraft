using System;
using System.Linq;
using System.Security.Claims;

namespace Simucraft.Client.Common
{
    public static class SecurityExtensions
    {
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(id))
                throw new NullReferenceException("Invalid Id.");

            return Guid.Parse(id);
        }
    }
}
