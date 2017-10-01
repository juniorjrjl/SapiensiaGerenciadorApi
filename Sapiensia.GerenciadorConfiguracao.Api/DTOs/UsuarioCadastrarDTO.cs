using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sapiensia.GerenciadorConfiguracao.Api.DTOs
{
    public class UsuarioCadastrarDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Informa um nome")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Informe um e-mail.")]
        [EmailAddress(ErrorMessage ="Informe um e-mail válido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Informe uma senha")]
        [MinLength(length: 6 , ErrorMessage = "A senha deve ter no mínimo 6 caractéres")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Confirme a senha informada.")]
        [Compare(otherProperty: "Senha", ErrorMessage = "As senhas são diferentes.")]
        public string ConfirmaSenha { get; set; }
        public List<PermissaoDTO> Permissoes { get; set; }
    }
}