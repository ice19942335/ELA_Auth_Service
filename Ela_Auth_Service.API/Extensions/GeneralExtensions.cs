using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ELA_Auth_Service.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if(httpContext.User is null)
                return string.Empty;

            return httpContext.User.Claims.Single(x => x.Type == "userId").Value;
        }
    }
}
