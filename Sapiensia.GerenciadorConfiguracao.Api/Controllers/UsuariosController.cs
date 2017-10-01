using Microsoft.AspNet.Identity.EntityFramework;
using Sapiensia.GerenciadorConfiguracao.Api.AutoMapper;
using Sapiensia.GerenciadorConfiguracao.Api.Context;
using Sapiensia.GerenciadorConfiguracao.Api.DAOs;
using Sapiensia.GerenciadorConfiguracao.Api.DTOs;
using Sapiensia.GerenciadorConfiguracao.Api.exceptions;
using Sapiensia.GerenciadorConfiguracao.Api.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sapiensia.GerenciadorConfiguracao.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        UsuarioDAO usuarioDAO = new UsuarioDAO(new SapiensiaDbContext());
        PermissaoDAO permissaoDAO = new PermissaoDAO(new SapiensiaDbContext());

        public IHttpActionResult Get()
        {
            IEnumerable<IdentityUser> usuarios = usuarioDAO.Selecionar();
            UsuariosPermissoesDTO dto = new UsuariosPermissoesDTO
            {
                Usuarios = AutoMapperManager.Instance.Mapper.Map<IEnumerable<IdentityUser>, IEnumerable<UsuarioAtualizarDTO>>(usuarios).ToList(),
                Permissoes = AutoMapperManager.Instance.Mapper.Map<IEnumerable<IdentityRole>, IEnumerable<PermissaoDTO>>(permissaoDAO.Selecionar()).ToList()
            };
            for (int i =0; i < dto.Usuarios.Count; i++)
            {
                List<String> nomePermissoes = new List<string>();
                usuarios.ToList()[i].Roles.ToList().ForEach(r => nomePermissoes.Add(r.RoleId));
                List<IdentityRole> roles = permissaoDAO.Selecionar(c => nomePermissoes.Contains(c.Id)).ToList();
                dto.Usuarios[i].Permissoes = AutoMapperManager.Instance.Mapper.Map<List<IdentityRole>, List<PermissaoDTO>>(roles);
            }
            return Ok(dto);
        }

        [FiltroValidacao]
        public IHttpActionResult Post([FromBody]UsuarioCadastrarDTO dto)
        {
            try
            {
                IdentityUser usuario = AutoMapperManager.Instance.Mapper.Map<UsuarioCadastrarDTO, IdentityUser>(dto);
                List<IdentityRole> permissoesConcedidas = AutoMapperManager.Instance.Mapper.Map<List<PermissaoDTO>, List<IdentityRole>>(dto.Permissoes);
                usuario = usuarioDAO.Cadastrar(usuario, permissoesConcedidas);
                dto = AutoMapperManager.Instance.Mapper.Map<IdentityUser, UsuarioCadastrarDTO>(usuario);
                dto.Permissoes = AutoMapperManager.Instance.Mapper.Map<List<IdentityRole>, List<PermissaoDTO>>(permissoesConcedidas);
                return Created($"Request.RequestUri/", dto);
            }catch(UsuarioException uEx)
            {
                return Content(HttpStatusCode.BadRequest, new { erro = uEx.Message});
            } catch(Exception ex)
            {
                return InternalServerError();
            }
        }

        [FiltroValidacao]
        public IHttpActionResult Put([FromBody]UsuarioAtualizarDTO dto)
        {
            try
            {
                IdentityUser usuario = AutoMapperManager.Instance.Mapper.Map<UsuarioAtualizarDTO, IdentityUser>(dto);
                List<IdentityRole> permissoesConcedidas = AutoMapperManager.Instance.Mapper.Map<List<PermissaoDTO>, List<IdentityRole>>(dto.Permissoes);
                List<string> idsPermissoesConcedidas = new List<string>();
                permissoesConcedidas.ForEach(p => idsPermissoesConcedidas.Add(p.Id));
                List<IdentityRole> permissoesRemovidas = permissaoDAO.Selecionar(p => !idsPermissoesConcedidas.Contains(p.Id)).ToList();
                usuario = usuarioDAO.Atualizar(usuario, permissoesConcedidas, permissoesRemovidas);
                dto = AutoMapperManager.Instance.Mapper.Map<IdentityUser, UsuarioAtualizarDTO>(usuario);
                dto.Permissoes = AutoMapperManager.Instance.Mapper.Map<List<IdentityRole>, List<PermissaoDTO>>(permissoesConcedidas);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Delete([FromBody]UsuarioCadastrarDTO dto)
        {
            try
            {
                IdentityUser usuario = AutoMapperManager.Instance.Mapper.Map<UsuarioCadastrarDTO, IdentityUser>(dto);
                usuarioDAO.Excluir(usuario);
                return Ok();
            }
            catch (UsuarioException uEx)
            {
                return Content(HttpStatusCode.BadRequest, new { erro = uEx.Message });
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

    }
}
