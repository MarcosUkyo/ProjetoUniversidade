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
    public class HistoricoController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Historico>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_historico_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(MapHistorico(rd));
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            CarregarSelects();
            return View(new Historico());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Historico model)
        {
            if (!ModelState.IsValid) { CarregarSelects(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_historico_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_nota",           model.Nota);
            cmd.Parameters.AddWithValue("p_frequencia_pct", model.FrequenciaPct);
            cmd.Parameters.AddWithValue("p_situacao",       model.Situacao);
            cmd.Parameters.AddWithValue("p_id_aluno",       model.IdAluno);
            cmd.Parameters.AddWithValue("p_id_turma",       model.IdTurma);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Histórico registrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var model = ObterPorId(id);
            if (model == null) return NotFound();
            CarregarSelects();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Editar(Historico model)
        {
            if (!ModelState.IsValid) { CarregarSelects(); return View(model); }
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_historico_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",             model.IdHistorico);
            cmd.Parameters.AddWithValue("p_nota",           model.Nota);
            cmd.Parameters.AddWithValue("p_frequencia_pct", model.FrequenciaPct);
            cmd.Parameters.AddWithValue("p_situacao",       model.Situacao);
            cmd.Parameters.AddWithValue("p_id_aluno",       model.IdAluno);
            cmd.Parameters.AddWithValue("p_id_turma",       model.IdTurma);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Histórico atualizado!";
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
            using var cmd = new MySqlCommand("sp_historico_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Histórico excluído!";
            return RedirectToAction(nameof(Index));
        }

        private Historico? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_historico_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return MapHistorico(rd);
        }

        private void CarregarSelects()
        {
            var alunos = new List<SelectListItem>();
            var turmas = new List<SelectListItem>();

            using var conn = _db.GetConnection();

            using (var cmd = new MySqlCommand("sp_aluno_listar", conn)
                { CommandType = CommandType.StoredProcedure })
            using (var rd = cmd.ExecuteReader())
                while (rd.Read())
                    alunos.Add(new SelectListItem
                    {
                        Value = rd.GetInt32("id_aluno").ToString(),
                        Text  = $"{rd.GetString("ra")} – {rd.GetString("nome")}"
                    });

            using (var cmd2 = new MySqlCommand("sp_turma_listar", conn)
                { CommandType = CommandType.StoredProcedure })
            using (var rd2 = cmd2.ExecuteReader())
                while (rd2.Read())
                    turmas.Add(new SelectListItem
                    {
                        Value = rd2.GetInt32("id_turma").ToString(),
                        Text  = $"{rd2["disciplina_codigo"]} – {rd2["disciplina_nome"]} ({rd2.GetString("semestre")})"
                    });

            ViewBag.Alunos = alunos;
            ViewBag.Turmas = turmas;
        }

        private static Historico MapHistorico(MySqlDataReader rd) => new Historico
        {
            IdHistorico    = rd.GetInt32("id_historico"),
            Nota           = rd.GetDecimal("nota"),
            FrequenciaPct  = rd.GetDecimal("frequencia_pct"),
            Situacao       = rd.GetString("situacao"),
            IdAluno        = rd.GetInt32("id_aluno"),
            AlunoNome      = rd["aluno_nome"] as string,
            AlunoRa        = rd["ra"] as string,
            IdTurma        = rd.GetInt32("id_turma"),
            Semestre       = rd["semestre"] as string,
            DisciplinaNome = rd["disciplina_nome"] as string
        };
    }
}
