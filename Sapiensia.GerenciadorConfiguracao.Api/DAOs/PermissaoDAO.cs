using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Sapiensia.GerenciadorConfiguracao.Api.Context;
using Sapiensia.GerenciadorConfiguracao.Api.exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.DAOs
{
    public class PermissaoDAO
    {
        private SapiensiaDbContext _context;

        public PermissaoDAO(SapiensiaDbContext contexto) 
        {
            _context = contexto;
        }

        public IdentityRole Cadastrar(IdentityRole permissao)
        {
            var roleStore = new RoleStore<IdentityRole>(_context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            try
            {
                AvaliarRetorno(roleManager.Create(permissao));
                return permissao;
            }
            catch (PermissaoException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<IdentityRole> Selecionar(Expression<Func<IdentityRole, bool>> where = null)
        {
            return (where == null )? _context.Roles : _context.Roles.Where(where);
        }

        public IdentityRole SelecionarUnico(Expression<Func<IdentityRole, bool>> where)
        {
            return _context.Roles.Where(where).FirstOrDefault();
        }

        private void AvaliarRetorno(IdentityResult resultado)
        {
            if (!resultado.Succeeded)
            {
                throw new PermissaoException(PrepararListaErros(resultado));
            }

        }

        private string PrepararListaErros(IdentityResult resultado)
        {
            if ((resultado.Errors == null) || (!resultado.Errors.Any()))
            {
                return "";
            }
            else
            {
                StringBuilder erros = new StringBuilder();
                resultado.Errors.ToList().ForEach(e => erros.Append(e));
                return erros.ToString();
            }
        }

    }
}