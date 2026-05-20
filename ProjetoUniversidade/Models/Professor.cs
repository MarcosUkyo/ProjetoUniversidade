using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoUniversidade.Models
{
    public class Professor
    {
        [Key]
        public int IdProfessor { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "A titulação é obrigatória")]
        [StringLength(30)]
        public string Titulacao { get; set; }

        [ForeignKey("Departamento")]
        public int IdDepto { get; set; }
        public virtual Departamento Departamento { get; set; }

        // Relacionamentos
        public virtual ICollection<Turma> Turmas { get; set; }
    }
}
