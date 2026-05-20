namespace ProjetoUniversidade.Models
{
    public class Usuarios
    {
        public int id {  get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public int senha_hash { get; set; }
        public int ativo { get; set; }
        public string criando_em { get; set; }
    }
}
