using Microsoft.AspNet.Identity.EntityFramework;
using Sapiensia.GerenciadorConfiguracao.Api.Context;
using Sapiensia.GerenciadorConfiguracao.Api.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sapiensia.GerenciadorConfiguracao.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            VerificarPermissoes();
            VerificarAdmin();
            VerificaPermissoesAdmin();
        }

        public void VerificarPermissoes()
        {
            PermissaoDAO dao = new PermissaoDAO(new SapiensiaDbContext());
            List<IdentityRole> permissoes = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "CADASTRAR_USUARIO"
                },

                new IdentityRole
                {
                    Name = "ATUALIZAR_USUARIO"
                },
                new IdentityRole
                {
                    Name = "EXCLUIR_USUARIO"
                }
            };
            foreach(IdentityRole p in permissoes)
            {
                if (dao.SelecionarUnico(r => r.Name == p.Name) == null)
                {
                    dao.Cadastrar(p);
                }
            }
        }

        public void VerificarAdmin()
        {
            UsuarioDAO dao = new UsuarioDAO(new SapiensiaDbContext());
            var usuarioAdmin = dao.SelecionarUnico(u => u.UserName == "admin");
            if (usuarioAdmin == null)
            {
                usuarioAdmin = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@admin.com.br",
                    PasswordHash = "password"
                };
                dao.Cadastrar(usuarioAdmin);
            }
        }

        public void VerificaPermissoesAdmin()
        {
            UsuarioDAO dao = new UsuarioDAO(new SapiensiaDbContext());
            var usuarioAdmin = dao.SelecionarUnico( u => u.UserName == "admin");
            if (usuarioAdmin.Roles.Count < 3)
            {
                PermissaoDAO permissaoDAO = new PermissaoDAO(new SapiensiaDbContext());

                List<string> idsPermissoes = new List<string>();
                usuarioAdmin.Roles.ToList().ForEach(idR => idsPermissoes.Add(idR.RoleId));
                List<IdentityRole> permissoes = (usuarioAdmin.Roles.Count == 0) ?
                    permissaoDAO.Selecionar().ToList() :
                    permissaoDAO.Selecionar(r => !idsPermissoes.Contains(r.Id)).ToList();

                List<string> nomePermissoes = new List<string>();
                permissoes.ForEach(p => nomePermissoes.Add(p.Name));
                dao.ConcederPermissoes(usuarioAdmin.Id, nomePermissoes);

            }
        }

    }
}
