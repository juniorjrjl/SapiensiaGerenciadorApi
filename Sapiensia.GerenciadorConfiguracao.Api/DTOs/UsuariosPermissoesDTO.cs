using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.DTOs
{
    public class UsuariosPermissoesDTO
    {
        public List<UsuarioAtualizarDTO> Usuarios { get; set; }
        public List<PermissaoDTO> Permissoes { get; set; }
    }
}