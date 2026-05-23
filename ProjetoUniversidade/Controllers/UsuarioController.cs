using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoUniversidade.Data;
using ProjetoUniversidade.Filters;
using ProjetoUniversidade.Models;
using System.Collections.Generic;
using System.Data;

namespace ProjetoUniversidade.Controllers
{
    [SessionAuthorize(RoleAnyOf = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Usuario>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_usuario_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new Usuario
                {
                    Id        = rd.GetInt32("id"),
                    Nome      = rd.GetString("nome"),
                    Email     = rd.GetString("email"),
                    Role      = rd.GetString("role"),
                    Ativo     = rd.GetBoolean("ativo"),
                    CriadoEm = rd.GetDateTime("criado_em")
                });
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar() => View(new Usuario());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Usuario model)
        {
            if (string.IsNullOrWhiteSpace(model.SenhaHash))
                ModelState.AddModelError("SenhaHash", "A senha é obrigatória para novo usuário.");
            if (!ModelState.IsValid) return View(model);

            var hash = BCrypt.Net.BCrypt.HashPassword(model.SenhaHash);

            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_usuario_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nome",       model.Nome);
            cmd.Parameters.AddWithValue("p_email",      model.Email);
            cmd.Parameters.AddWithValue("p_senha_hash", hash);
            cmd.Parameters.AddWithValue("p_role",       model.Role);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Usuário criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            model.SenhaHash = null; // não expõe o hash
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(Usuario model)
        {
            // Senha é opcional na edição
            ModelState.Remove("SenhaHash");
            if (!ModelState.IsValid) return View(model);

            using var conn = _db.GetConnection();

            // Se digitou nova senha, atualiza o hash
            if (!string.IsNullOrWhiteSpace(model.SenhaHash))
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(model.SenhaHash);
                using var cmdSenha = new MySqlCommand(
                    "UPDATE Usuarios SET senha_hash=@h WHERE id=@id", conn);
                cmdSenha.Parameters.AddWithValue("@h",  hash);
                cmdSenha.Parameters.AddWithValue("@id", model.Id);
                cmdSenha.ExecuteNonQuery();
            }

            using var cmd = new MySqlCommand("sp_usuario_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",    model.Id);
            cmd.Parameters.AddWithValue("p_nome",  model.Nome);
            cmd.Parameters.AddWithValue("p_email", model.Email);
            cmd.Parameters.AddWithValue("p_role",  model.Role);
            cmd.Parameters.AddWithValue("p_ativo", model.Ativo ? 1 : 0);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Usuário atualizado!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Excluir"), ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_usuario_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Usuário excluído!";
            return RedirectToAction(nameof(Index));
        }

        private Usuario? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_usuario_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Usuario
            {
                Id        = rd.GetInt32("id"),
                Nome      = rd.GetString("nome"),
                Email     = rd.GetString("email"),
                Role      = rd.GetString("role"),
                Ativo     = rd.GetBoolean("ativo"),
                CriadoEm = rd.GetDateTime("criado_em")
            };
        }
    }
}
