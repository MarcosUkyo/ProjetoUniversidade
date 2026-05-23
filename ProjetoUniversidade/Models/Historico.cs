using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Historico
    {
        public int IdHistorico { get; set; }

        [Required(ErrorMessage = "A nota é obrigatória")]
        [Range(0, 10, ErrorMessage = "A nota deve ser entre 0 e 10")]
        [Display(Name = "Nota")]
        public decimal Nota { get; set; }

        [Required(ErrorMessage = "A frequência é obrigatória")]
        [Range(0, 100, ErrorMessage = "A frequência deve ser entre 0 e 100")]
        [Display(Name = "Frequência (%)")]
        public decimal FrequenciaPct { get; set; }

        [Required(ErrorMessage = "A situação é obrigatória")]
        [StringLength(20)]
        [Display(Name = "Situação")]
        public string Situacao { get; set; } = "";

        [Display(Name = "Aluno")]
        public int IdAluno { get; set; }
        public string? AlunoNome { get; set; }
        public string? AlunoRa { get; set; }

        [Display(Name = "Turma")]
        public int IdTurma { get; set; }
        public string? Semestre { get; set; }
        public string? DisciplinaNome { get; set; }
    }
}
