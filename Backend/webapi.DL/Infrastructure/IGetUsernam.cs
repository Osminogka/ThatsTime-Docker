using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webapi.DL.Infrastructure
{
    public interface IGetUsername
    {
        string getUserName(HttpContext ctx);
    }
}
