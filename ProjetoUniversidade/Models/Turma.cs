using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoUniversidade.Models
{
    public class Turma
    {
        [Key]
        public int IdTurma { get; set; }

        [Required(ErrorMessage = "O semestre é obrigatório")]
        [StringLength(10)]
        public string Semestre { get; set; }

        [ForeignKey("Disciplina")]
        public int IdDisciplina { get; set; }
        public virtual Disciplina Disciplina { get; set; }

        [ForeignKey("Professor")]
        public int IdProfessor { get; set; }
        public virtual Professor Professor { get; set; }

        // Relacionamentos
        public virtual ICollection<Historico> Historicos { get; set; }
    }
}
