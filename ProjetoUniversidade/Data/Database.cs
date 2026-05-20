using MySql.Data.MySqlClient;

namespace ProjetoUniversidade.Data
{
    public class Database
    {
        private readonly string connectionString = "server=localhost;port=3306;database=bduniversidade;user=root;password=12345678;";

        public MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
