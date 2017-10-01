using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.Context
{
    public class SapiensiaDbContext : IdentityDbContext<IdentityUser>
    {
        public SapiensiaDbContext()
            :base("SapiensiaDbContext")
        {
                
        }
    }
}