using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using ProjetoUniversidade.Data;
using ProjetoUniversidade.Filters;
using ProjetoUniversidade.Models;
using System.Collections.Generic;
using System.Data;

namespace ProjetoUniversidade.Controllers
{
    [SessionAuthorize]
    public class ProfessoresController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Professor>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_professor_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(MapProfessor(rd));
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            CarregarDepartamentos();
            return View(new Professor());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Professor model)
        {
            if (!ModelState.IsValid) { CarregarDepartamentos(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_professor_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nome",      model.Nome);
            cmd.Parameters.AddWithValue("p_cpf",       model.Cpf);
            cmd.Parameters.AddWithValue("p_titulacao", model.Titulacao);
            cmd.Parameters.AddWithValue("p_id_depto",  model.IdDepto);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Professor cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            CarregarDepartamentos();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(Professor model)
        {
            if (!ModelState.IsValid) { CarregarDepartamentos(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_professor_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",        model.IdProfessor);
            cmd.Parameters.AddWithValue("p_nome",      model.Nome);
            cmd.Parameters.AddWithValue("p_cpf",       model.Cpf);
            cmd.Parameters.AddWithValue("p_titulacao", model.Titulacao);
            cmd.Parameters.AddWithValue("p_id_depto",  model.IdDepto);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Professor atualizado!";
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
            using var cmd = new MySqlCommand("sp_professor_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Professor excluído!";
            return RedirectToAction(nameof(Index));
        }

        // ---------- helpers ----------
        private Professor? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_professor_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return MapProfessor(rd);
        }

        private void CarregarDepartamentos()
        {
            var lista = new List<SelectListItem>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_departamento_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new SelectListItem
                {
                    Value = rd.GetInt32("id_depto").ToString(),
                    Text  = $"{rd.GetString("sigla")} – {rd.GetString("nome")}"
                });
            ViewBag.Departamentos = lista;
        }

        private static Professor MapProfessor(MySqlDataReader rd) => new Professor
        {
            IdProfessor = rd.GetInt32("id_professor"),
            Nome        = rd.GetString("nome"),
            Cpf         = rd.GetString("cpf"),
            Titulacao   = rd.GetString("titulacao"),
            IdDepto     = rd.GetInt32("id_depto"),
            DeptoNome   = rd["depto_nome"] as string
        };
    }
}
