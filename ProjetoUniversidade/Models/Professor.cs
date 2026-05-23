using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Professor
    {
        public int IdProfessor { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = "";

        [Required(ErrorMessage = "A titulação é obrigatória")]
        [StringLength(30)]
        [Display(Name = "Titulação")]
        public string Titulacao { get; set; } = "";

        [Display(Name = "Departamento")]
        public int IdDepto { get; set; }

        public string? DeptoNome { get; set; }
    }
}
