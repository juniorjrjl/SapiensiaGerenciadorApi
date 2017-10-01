using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using Sapiensia.GerenciadorConfiguracao.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.AutoMapper
{
    public class AutoMapperManager
    {
        private static readonly Lazy<AutoMapperManager> _instance = new Lazy<AutoMapperManager>(() =>
        {
            return new AutoMapperManager();
        });

        public static AutoMapperManager Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private MapperConfiguration _config;

        public IMapper Mapper
        {
            get
            {
                return _config.CreateMapper();
            }
        }

        private AutoMapperManager()
        {
            _config = new MapperConfiguration((cfg) =>
            {

                cfg.CreateMap<UsuarioCadastrarDTO, IdentityUser>()
                    .ForMember(iu => iu.Id, opt => opt.MapFrom(u => u.Id))
                    .ForMember(iu => iu.UserName, opt => opt.MapFrom(u => u.Nome))
                    .ForMember(iu => iu.Email, opt => opt.MapFrom(u => u.Email))
                    .ForMember(iu => iu.PasswordHash, opt => opt.MapFrom(u => u.Senha));
                cfg.CreateMap<IdentityUser, UsuarioCadastrarDTO>()
                    .ForMember(u => u.Id, opt => opt.MapFrom(iu => iu.Id))
                    .ForMember(u => u.Nome, opt => opt.MapFrom(iu => iu.UserName))
                    .ForMember(u => u.Email, opt => opt.MapFrom(iu => iu.Email))
                    .ForMember(u => u.Senha, opt => opt.MapFrom(iu => iu.PasswordHash))
                    .ForMember(u => u.ConfirmaSenha, opt => opt.MapFrom(iu => iu.PasswordHash));

                cfg.CreateMap<UsuarioAtualizarDTO, IdentityUser>()
                    .ForMember(iu => iu.Id, opt => opt.MapFrom(u => u.Id))
                    .ForMember(iu => iu.UserName, opt => opt.MapFrom(u => u.Nome))
                    .ForMember(iu => iu.Email, opt => opt.MapFrom(u => u.Email))
                    .ForMember(iu => iu.PasswordHash, opt => opt.MapFrom(u => u.Senha));
                cfg.CreateMap<IdentityUser, UsuarioAtualizarDTO>()
                    .ForMember(u => u.Id, opt => opt.MapFrom(iu => iu.Id))
                    .ForMember(u => u.Nome, opt => opt.MapFrom(iu => iu.UserName))
                    .ForMember(u => u.Email, opt => opt.MapFrom(iu => iu.Email))
                    .ForMember(u => u.Senha, opt => opt.MapFrom(iu => iu.PasswordHash))
                    .ForMember(u => u.ConfirmaSenha, opt => opt.MapFrom(iu => iu.PasswordHash));

                cfg.CreateMap<PermissaoDTO, IdentityRole>()
                    .ForMember(ir => ir.Name, opt => opt.MapFrom(p => p.Nome))
                    .ForMember(ir => ir.Id, opt => opt.MapFrom(p => p.Id));
                cfg.CreateMap<IdentityRole, PermissaoDTO>()
                    .ForMember(p => p.Nome, opt => opt.MapFrom(ir => ir.Name))
                    .ForMember(p => p.Id, opt => opt.MapFrom(ir => ir.Id));
                
            });
        }

    }
}