using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Departamento
    {
        public int IdDepto { get; set; }

        [Required(ErrorMessage = "O nome do departamento é obrigatório")]
        [StringLength(80)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "A sigla é obrigatória")]
        [StringLength(10)]
        [Display(Name = "Sigla")]
        public string Sigla { get; set; } = "";
    }
}
