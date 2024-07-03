using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace webapi.DL.Infrastructure
{
    public class GetUsername: IGetUsername
    {
        public string getUserName(HttpContext ctx)
        {
            return ctx.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        }
    }
}
