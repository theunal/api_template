using Core.Models;
using System.Security.Claims;

namespace Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static IEnumerable<OperationClaimEnum> ClaimRoles(this ClaimsPrincipal claimsPrincipal)
        {
            var roles = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(roles) is false)
                return  System.Text.Json.JsonSerializer.Deserialize<IEnumerable<int>>(roles)!.Select(x => (OperationClaimEnum)x);
            
            return Array.Empty<OperationClaimEnum>();
        }

        public static long? GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "UsId")?.Value;
            if (id is null) return null;

            return long.Parse(id);
        }

        public static string? GetSessionHash(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "NSessionHash")?.Value;
    }
}
