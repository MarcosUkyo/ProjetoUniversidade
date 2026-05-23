using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Turma
    {
        public int IdTurma { get; set; }

        [Required(ErrorMessage = "O semestre é obrigatório")]
        [StringLength(10)]
        [Display(Name = "Semestre")]
        public string Semestre { get; set; } = "";

        [Display(Name = "Disciplina")]
        public int IdDisciplina { get; set; }

        public string? DisciplinaNome { get; set; }
        public string? DisciplinaCodigo { get; set; }

        [Display(Name = "Professor")]
        public int IdProfessor { get; set; }

        public string? ProfessorNome { get; set; }
    }
}
