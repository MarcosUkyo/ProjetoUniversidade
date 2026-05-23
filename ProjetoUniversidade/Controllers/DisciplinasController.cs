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
    public class DisciplinasController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Disciplina>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_disciplina_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(MapDisciplina(rd));
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            CarregarDepartamentos();
            return View(new Disciplina());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Disciplina model)
        {
            if (!ModelState.IsValid) { CarregarDepartamentos(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_disciplina_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_codigo",        model.Codigo);
            cmd.Parameters.AddWithValue("p_nome",          model.Nome);
            cmd.Parameters.AddWithValue("p_carga_horaria", model.CargaHoraria);
            cmd.Parameters.AddWithValue("p_id_depto",      model.IdDepto);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Disciplina cadastrada com sucesso!";
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
        public IActionResult Editar(Disciplina model)
        {
            if (!ModelState.IsValid) { CarregarDepartamentos(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_disciplina_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",            model.IdDisciplina);
            cmd.Parameters.AddWithValue("p_codigo",        model.Codigo);
            cmd.Parameters.AddWithValue("p_nome",          model.Nome);
            cmd.Parameters.AddWithValue("p_carga_horaria", model.CargaHoraria);
            cmd.Parameters.AddWithValue("p_id_depto",      model.IdDepto);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Disciplina atualizada!";
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
            using var cmd = new MySqlCommand("sp_disciplina_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Disciplina excluída!";
            return RedirectToAction(nameof(Index));
        }

        private Disciplina? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_disciplina_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return MapDisciplina(rd);
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

        private static Disciplina MapDisciplina(MySqlDataReader rd) => new Disciplina
        {
            IdDisciplina  = rd.GetInt32("id_disciplina"),
            Codigo        = rd.GetString("codigo"),
            Nome          = rd.GetString("nome"),
            CargaHoraria  = rd.GetInt32("carga_horaria"),
            IdDepto       = rd.GetInt32("id_depto"),
            DeptoNome     = rd["depto_nome"] as string
        };
    }
}
