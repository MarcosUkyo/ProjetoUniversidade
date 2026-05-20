using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoUniversidade.Models
{
    public class Disciplina
    {
        [Key]
        public int IdDisciplina { get; set; }

        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(10)]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A carga horária é obrigatória")]
        public int CargaHoraria { get; set; }

        [ForeignKey("Departamento")]
        public int IdDepto { get; set; }
        public virtual Departamento Departamento { get; set; }

        // Relacionamentos
        public virtual ICollection<Turma> Turmas { get; set; }
    }
}
