using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Aluno
    {
        [Key]
        public int IdAluno { get; set; }

        [Required(ErrorMessage = "O RA é obrigatório")]
        [StringLength(20)]
        public string Ra { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "A data de ingresso é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime DataIngresso { get; set; }

        // Relacionamentos
        public virtual ICollection<Historico> Historicos { get; set; }
    }
}
