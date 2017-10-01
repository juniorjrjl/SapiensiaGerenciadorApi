using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Sapiensia.GerenciadorConfiguracao.Api.Context;
using Sapiensia.GerenciadorConfiguracao.Api.exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.DAOs
{
    public class UsuarioDAO
    {
        private SapiensiaDbContext _context;

        public UsuarioDAO(SapiensiaDbContext contexto)
        {
            _context = contexto;
        }

        public IdentityUser VerificarLogin(string email, string senha)
        {
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore);
            return userManager.Find(email, senha);
        }

        public void ConcederPermissoes(string idUsuario, List<String> permissoes)
        {
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore);
            try
            {
                List<string> permissoesRelacionadas = new List<string>();
                foreach (string p in permissoes)
                {
                    if (userManager.IsInRole(idUsuario, p))
                    {
                        permissoesRelacionadas.Add(p);
                    }
                }
                permissoes.RemoveAll(p => permissoesRelacionadas.Contains(p));
                if (permissoes.Any())
                {
                    AvaliarRetorno(userManager.AddToRoles(idUsuario, permissoes.ToArray()));
                }
            }
            catch (UsuarioException ex)
            {
                throw ex;
            }
        }

        public void RemoverPermissoes(string idUsuario, List<String> permissoes)
        {
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore);
            try
            {
                List<string> permissoesNaoRelacionadas = new List<string>();
                foreach(string p in permissoes)
                {
                    if (!userManager.IsInRole(idUsuario, p))
                    {
                        permissoesNaoRelacionadas.Add(p);
                    }
                }
                permissoes.RemoveAll(p => permissoesNaoRelacionadas.Contains(p));
                if (permissoes.Any())
                {
                    AvaliarRetorno(userManager.RemoveFromRoles(idUsuario, permissoes.ToArray()));
                }
            }
            catch (UsuarioException ex)
            {
                throw ex;
            }
        }

        public IdentityUser Cadastrar(IdentityUser usuario, List<IdentityRole> permissoes = null)
        {
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore);
            var identityUser = new IdentityUser
            {
                UserName = usuario.UserName,
                Email = usuario.Email
            };
            try
            {
                AvaliarRetorno(userManager.Create(identityUser, usuario.PasswordHash));
                if (permissoes != null)
                {
                    List<String> descricaoPermissao = new List<string>();
                    permissoes.ForEach(p => descricaoPermissao.Add(p.Name));
                    ConcederPermissoes(identityUser.Id, descricaoPermissao);
                }
                return identityUser;
            }
            catch (UsuarioException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<IdentityUser> Selecionar(Expression<Func<IdentityUser, bool>> where = null)
        {
            return (where == null) ? _context.Users.Include("Roles").ToList().OrderBy(iu => iu.UserName).ToList()
                : _context.Users.Include("Roles").OrderBy(iu => iu.UserName).Where(where).ToList();
        }

        public IdentityUser SelecionarUnico(Expression<Func<IdentityUser, bool>> where)
        {
            return _context.Users.Include("Roles").FirstOrDefault(where);
        }

        public void Excluir(IdentityUser usuario)
        {
            var userStore = new UserStore<IdentityUser>(_context);
            var userManager = new UserManager<IdentityUser>(userStore);
            try
            {
                _context.Users.Attach(usuario);
                AvaliarRetorno(userManager.Delete(usuario));
            }
            catch(UsuarioException ex)
            {
                throw ex;
            }

        }

        public IdentityUser Atualizar(IdentityUser usuario, List<IdentityRole> permissoesConceder = null, List<IdentityRole> permissoesRemover = null)
        {
            try
            {
                _context.Users.Attach(usuario);
                _context.Entry(usuario).Property(u => u.UserName).IsModified = true;
                _context.Entry(usuario).Property(u => u.Email).IsModified = true;
                if ((usuario.PasswordHash != null) && (usuario.PasswordHash != String.Empty))
                {
                    var userStore = new UserStore<IdentityUser>(_context);
                    var userManager = new UserManager<IdentityUser>(userStore);
                    usuario.PasswordHash = userManager.PasswordHasher.HashPassword(usuario.PasswordHash);
                    _context.Entry(usuario).Property(u => u.PasswordHash).IsModified = true;
                }
                if (permissoesRemover != null)
                {
                    List<String> descricaoPermissao = new List<string>();
                    permissoesRemover.ForEach(p => descricaoPermissao.Add(p.Name));
                    RemoverPermissoes(usuario.Id, descricaoPermissao);
                }
                if (permissoesConceder != null)
                {
                    List<String> descricaoPermissao = new List<string>();
                    permissoesConceder.ForEach(p => descricaoPermissao.Add(p.Name));
                    ConcederPermissoes(usuario.Id, descricaoPermissao);
                }
                _context.SaveChanges();
                return usuario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AvaliarRetorno(IdentityResult resultado)
        {
            if (!resultado.Succeeded)
            {
                throw new UsuarioException(PrepararListaErros(resultado));
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