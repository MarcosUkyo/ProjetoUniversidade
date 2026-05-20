using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoUniversidade.Models
{
    public class Historico
    {
        [Key]
        public int IdHistorico { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória")]
        [Range(0, 10, ErrorMessage = "A nota deve ser entre 0 e 10")]
        public decimal Nota { get; set; }

        [Required(ErrorMessage = "A frequência é obrigatória")]
        [Range(0, 100, ErrorMessage = "A frequência deve ser entre 0 e 100")]
        public decimal FrequenciaPct { get; set; }

        [Required(ErrorMessage = "A situação é obrigatória")]
        [StringLength(20)]
        public string Situacao { get; set; }

        [ForeignKey("Aluno")]
        public int IdAluno { get; set; }
        public virtual Aluno Aluno { get; set; }

        [ForeignKey("Turma")]
        public int IdTurma { get; set; }
        public virtual Turma Turma { get; set; }
    }
}
