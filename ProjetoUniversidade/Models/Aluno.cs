using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Aluno
    {
        public int IdAluno { get; set; }

        [Required(ErrorMessage = "O RA é obrigatório")]
        [StringLength(20)]
        [Display(Name = "RA")]
        public string Ra { get; set; } = "";

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = "";

        [Required(ErrorMessage = "A data de ingresso é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Ingresso")]
        public DateTime DataIngresso { get; set; }
    }
}
