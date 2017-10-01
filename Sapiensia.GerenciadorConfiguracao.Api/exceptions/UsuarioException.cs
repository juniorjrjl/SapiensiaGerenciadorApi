using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.exceptions
{
    public class UsuarioException : Exception
    {
        public UsuarioException(String mensagem)
            :base(mensagem)
        {
                
        }
    }
}