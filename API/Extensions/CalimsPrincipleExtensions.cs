using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
namespace API.Extensions
{
    public static class CalimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return  user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return  int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}