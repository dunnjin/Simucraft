using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Security.Claims;

namespace Simucraft.Server.Common
{
    public static class ApiExtensions
    {
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
                throw new InvalidOperationException("Invalid Id.");

            return Guid.Parse(id);
        }

        public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            var name = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return name;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
            return email;
        }

        public static Guid GetGameId(this HttpRequest httpRequest) =>
            Guid.Parse(httpRequest.Headers["GameId"]);
    }
}
