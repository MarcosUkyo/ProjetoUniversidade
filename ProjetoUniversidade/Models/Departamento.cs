using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetoUniversidade.Models
{
    public class Departamento
    {
        [Key]
        public int IdDepto { get; set; }

        [Required(ErrorMessage = "O nome do departamento é obrigatório")]
        [StringLength(80)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A sigla é obrigatória")]
        [StringLength(10)]
        public string Sigla { get; set; }

        // Relacionamentos
        public virtual ICollection<Professor> Professores { get; set; }
        public virtual ICollection<Disciplina> Disciplinas { get; set; }
    }
}
