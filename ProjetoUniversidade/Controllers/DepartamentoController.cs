using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoUniversidade.Data;
using ProjetoUniversidade.Filters;
using ProjetoUniversidade.Models;
using System.Collections.Generic;
using System.Data;

namespace ProjetoUniversidade.Controllers
{
    [SessionAuthorize]
    public class DepartamentoController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Departamento>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_departamento_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new Departamento
                {
                    IdDepto = rd.GetInt32("id_depto"),
                    Nome    = rd.GetString("nome"),
                    Sigla   = rd.GetString("sigla")
                });
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar() => View(new Departamento());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Departamento model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_departamento_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nome",  model.Nome);
            cmd.Parameters.AddWithValue("p_sigla", model.Sigla);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Departamento cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(Departamento model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_departamento_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",    model.IdDepto);
            cmd.Parameters.AddWithValue("p_nome",  model.Nome);
            cmd.Parameters.AddWithValue("p_sigla", model.Sigla);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Departamento atualizado!";
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
            using var cmd = new MySqlCommand("sp_departamento_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Departamento excluído!";
            return RedirectToAction(nameof(Index));
        }

        private Departamento? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_departamento_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Departamento
            {
                IdDepto = rd.GetInt32("id_depto"),
                Nome    = rd.GetString("nome"),
                Sigla   = rd.GetString("sigla")
            };
        }
    }
}
