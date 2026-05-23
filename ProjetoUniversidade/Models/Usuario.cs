using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [StringLength(100)]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [StringLength(255)]
        [Display(Name = "Senha")]
        public string? SenhaHash { get; set; }

        [Required(ErrorMessage = "O papel é obrigatório")]
        [StringLength(20)]
        [Display(Name = "Papel")]
        public string Role { get; set; } = "Aluno";

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Criado em")]
        public DateTime CriadoEm { get; set; }
    }
}
