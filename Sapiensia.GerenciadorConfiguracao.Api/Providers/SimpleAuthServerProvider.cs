using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Sapiensia.GerenciadorConfiguracao.Api.Context;
using Sapiensia.GerenciadorConfiguracao.Api.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.Providers
{
    public class SimpleAuthServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });
            UsuarioDAO usuarioDAO = new UsuarioDAO(new SapiensiaDbContext());
            PermissaoDAO permissaoDAO = new PermissaoDAO(new SapiensiaDbContext());
            var usuarioLogar = usuarioDAO.VerificarLogin(context.UserName, context.Password);
            if (usuarioLogar == null)
            {
                context.SetError("invalid_user_or_password", "Usuário e/ou senha incorretos.");
                return Task.FromResult<object>(null);
            }
            List<string> nomePermissoes = new List<string>();
            usuarioLogar.Roles.ToList().ForEach(p => nomePermissoes.Add(permissaoDAO.SelecionarUnico(r => r.Id == p.RoleId).Name));
            Dictionary<string, string> propriedade = new Dictionary<string, string>();
            nomePermissoes.ForEach(p => propriedade.Add(p,p));
            propriedade.Add("nomeUsuario", context.UserName);

            var props = new AuthenticationProperties(propriedade);            
            Claim claim = new Claim(ClaimTypes.Name, context.UserName);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim> { claim}, OAuthDefaults.AuthenticationType);
            var ticket = new AuthenticationTicket(claimsIdentity, props);
            context.Validated(ticket);
            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

    }
}