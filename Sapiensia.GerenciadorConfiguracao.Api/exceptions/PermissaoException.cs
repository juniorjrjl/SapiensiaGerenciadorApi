using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.exceptions
{
    public class PermissaoException : Exception
    {
        public PermissaoException(String mensagem)
            :base(mensagem)
        {
                
        }
    }
}