using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Disciplina
    {
        public int IdDisciplina { get; set; }

        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(10)]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = "";

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "A carga horária é obrigatória")]
        [Range(1, 9999)]
        [Display(Name = "Carga Horária (h)")]
        public int CargaHoraria { get; set; }

        [Display(Name = "Departamento")]
        public int IdDepto { get; set; }

        public string? DeptoNome { get; set; }
    }
}
