using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoUniversidade.Autenticacao;
using ProjetoUniversidade.Data;
using System.Data;

namespace ProjetoUniversidade.Controllers
{
    public class AuthController : Controller
    {
        private readonly Database _db = new Database();

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Se já está logado, vai direto para o início
            if (HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(string email, string senha, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Error = "Informe e-mail e senha.";
                return View();
            }

            using var conn = _db.GetConnection();
            using var cmd  = new MySqlCommand("sp_usuario_obter_por_email", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_email", email);
            using var rd = cmd.ExecuteReader();

            if (!rd.Read())
            {
                ViewBag.Error = "Usuário não encontrado.";
                return View();
            }

            var id        = rd.GetInt32("id");
            var nome      = rd.GetString("nome");
            var role      = rd.GetString("role");
            var ativo     = rd.GetBoolean("ativo");
            var senhaHash = rd["senha_hash"] as string ?? "";
            rd.Close();

            if (!ativo)
            {
                ViewBag.Error = "Usuário inativo. Contate o administrador.";
                return View();
            }

            bool senhaOk;
            try   { senhaOk = BCrypt.Net.BCrypt.Verify(senha, senhaHash); }
            catch { senhaOk = false; }

            if (!senhaOk)
            {
                ViewBag.Error = "Senha inválida.";
                return View();
            }

            // Salva sessão
            HttpContext.Session.SetInt32(SessionKeys.UserId,    id);
            HttpContext.Session.SetString(SessionKeys.UserName,  nome);
            HttpContext.Session.SetString(SessionKeys.UserEmail, email);
            HttpContext.Session.SetString(SessionKeys.UserRole,  role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AcessoNegado() => View();
    }
}
