using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjetoUniversidade.Data;
using ProjetoUniversidade.Filters;
using ProjetoUniversidade.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProjetoUniversidade.Controllers
{
    [SessionAuthorize]
    public class AlunosController : Controller
    {
        private readonly Database _db = new Database();

        public IActionResult Index()
        {
            var lista = new List<Aluno>();
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_aluno_listar", conn)
                { CommandType = CommandType.StoredProcedure };
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(MapAluno(rd));
            return View(lista);
        }

        [HttpGet]
        public IActionResult Criar() => View(new Aluno { DataIngresso = DateTime.Today });

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Criar(Aluno model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_aluno_criar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_ra",            model.Ra);
            cmd.Parameters.AddWithValue("p_nome",          model.Nome);
            cmd.Parameters.AddWithValue("p_cpf",           model.Cpf);
            cmd.Parameters.AddWithValue("p_data_ingresso", model.DataIngresso.Date);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Aluno cadastrado com sucesso!";
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
        public IActionResult Editar(Aluno model)
        {
            if (!ModelState.IsValid) return View(model);
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_aluno_editar", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id",            model.IdAluno);
            cmd.Parameters.AddWithValue("p_ra",            model.Ra);
            cmd.Parameters.AddWithValue("p_nome",          model.Nome);
            cmd.Parameters.AddWithValue("p_cpf",           model.Cpf);
            cmd.Parameters.AddWithValue("p_data_ingresso", model.DataIngresso.Date);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Aluno atualizado!";
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
            using var cmd = new MySqlCommand("sp_aluno_excluir", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            cmd.ExecuteNonQuery();
            TempData["ok"] = "Aluno excluído!";
            return RedirectToAction(nameof(Index));
        }

        private Aluno? ObterPorId(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new MySqlCommand("sp_aluno_obter", conn)
                { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("p_id", id);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return MapAluno(rd);
        }

        private static Aluno MapAluno(MySqlDataReader rd) => new Aluno
        {
            IdAluno       = rd.GetInt32("id_aluno"),
            Ra            = rd.GetString("ra"),
            Nome          = rd.GetString("nome"),
            Cpf           = rd.GetString("cpf"),
            DataIngresso  = rd.GetDateTime("data_ingresso")
        };
    }
}
