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
    public class TurmasController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Turma>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_turma_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(MapTurma(rd));
            return View(lista);
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult Criar()
        {
            CarregarSelects();
            return View(new Turma());
        }

        [HttpPost, ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult Criar(Turma model)
        {
            if (!ModelState.IsValid) { CarregarSelects(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_turma_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_semestre",      model.Semestre);
            cmd.Parameters.AddWithValue("p_id_disciplina", model.IdDisciplina);
            cmd.Parameters.AddWithValue("p_id_professor",  model.IdProfessor);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Turma cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult Editar(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            CarregarSelects();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult Editar(Turma model)
        {
            if (!ModelState.IsValid) { CarregarSelects(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_turma_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",            model.IdTurma);
            cmd.Parameters.AddWithValue("p_semestre",      model.Semestre);
            cmd.Parameters.AddWithValue("p_id_disciplina", model.IdDisciplina);
            cmd.Parameters.AddWithValue("p_id_professor",  model.IdProfessor);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Turma atualizada!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult Excluir(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Excluir"), ValidateAntiForgeryToken]
        [SessionAuthorize(RoleAnyOf = "Reitor,Gerente")]
        public IActionResult ExcluirConfirmado(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_turma_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Turma excluída!";
            return RedirectToAction(nameof(Index));
        }

        private Turma? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_turma_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return MapTurma(rd);
        }

        private void CarregarSelects()
        {
            var disciplinas = new List<SelectListItem>();
            var professores = new List<SelectListItem>();
            using var conn = _db.GetConnection();

            using (var cmd = new MySqlCommand("sp_disciplina_listar", conn)
                { CommandType = CommandType.StoredProcedure })
            using (var rd = cmd.ExecuteReader())
                while (rd.Read())
                    disciplinas.Add(new SelectListItem
                    {
                        Value = rd.GetInt32("id_disciplina").ToString(),
                        Text  = $"{rd.GetString("codigo")} – {rd.GetString("nome")}"
                    });

            using (var cmd2 = new MySqlCommand("sp_professor_listar", conn)
                { CommandType = CommandType.StoredProcedure })
            using (var rd2 = cmd2.ExecuteReader())
                while (rd2.Read())
                    professores.Add(new SelectListItem
                    {
                        Value = rd2.GetInt32("id_professor").ToString(),
                        Text  = rd2.GetString("nome")
                    });

            ViewBag.Disciplinas = disciplinas;
            ViewBag.Professores = professores;
        }

        private static Turma MapTurma(MySqlDataReader rd) => new Turma
        {
            IdTurma          = rd.GetInt32("id_turma"),
            Semestre         = rd.GetString("semestre"),
            IdDisciplina     = rd.GetInt32("id_disciplina"),
            DisciplinaNome   = rd["disciplina_nome"] as string,
            DisciplinaCodigo = rd["disciplina_codigo"] as string,
            IdProfessor      = rd.GetInt32("id_professor"),
            ProfessorNome    = rd["professor_nome"] as string
        };
    }
}
