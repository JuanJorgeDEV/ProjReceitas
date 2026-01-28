using Microsoft.AspNetCore.Mvc;
using ProjReceitas.BancoDados;
using ProjReceitas.Models;
using System.Data;
using System;

namespace ProjReceitas.Controllers
{
    public class IngredienteController : Controller
    {
        public IActionResult Selecionar()
        {
            try
            {
                Ingrediente o_Ingrediente = new Ingrediente();
                DataTable dtIngrediente = o_Ingrediente.SelecionarTodos();

                return View("SelecionarView", dtIngrediente);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao selecionar ingredientes: {ex.Message}";
                return View("SelecionarView");
            }
        }

        public IActionResult InserirExibir()
        {
            return View("InserirExibirView", new IngredienteViewModel());
        }

        public IActionResult InserirProcessar(IngredienteViewModel o_IngredienteVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Ingrediente o_Ingrediente = new Ingrediente();
                    o_Ingrediente.nome = o_IngredienteVM.Nome;

                    o_Ingrediente.Inserir();

                    TempData["MsgSucesso"] = "Ingrediente inserido com sucesso!";
                    return RedirectToAction("Selecionar");
                }

                return View("InserirExibirView", o_IngredienteVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao inserir ingrediente: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        public IActionResult ExcluirExibir(int idIngrediente)
        {
            try
            {
                Ingrediente o_Ingrediente = new Ingrediente();
                o_Ingrediente.id_ingrediente = idIngrediente;
                DataTable pesqIngrediente = o_Ingrediente.SelecionarPorID();

                if (pesqIngrediente == null || pesqIngrediente.Rows.Count == 0)
                {
                    TempData["MsgErro"] = "Ingrediente não encontrado.";
                    return RedirectToAction("Selecionar");
                }

                IngredienteViewModel o_IngredienteVM = new IngredienteViewModel();
                o_IngredienteVM.Id_ingrediente = idIngrediente;
                o_IngredienteVM.Nome = pesqIngrediente.Rows[0]["Nome"].ToString();

                if (pesqIngrediente.Rows[0]["Preco"] != DBNull.Value)
                    o_IngredienteVM.preco = Convert.ToDecimal(pesqIngrediente.Rows[0]["Preco"]);

                return View("ExcluirExibirView", o_IngredienteVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao exibir dados para exclusão do ingrediente: {ex.Message}";
                return RedirectToAction("Selecionar");
            }
        }

        public IActionResult ExcluirProcessar(IngredienteViewModel o_IngredienteVM)
        {
            try
            {
                Ingrediente o_Ingrediente = new Ingrediente();
                o_Ingrediente.id_ingrediente = o_IngredienteVM.Id_ingrediente;

                o_Ingrediente.Excluir();
                TempData["MsgSucesso"] = "Ingrediente excluído com sucesso!";

                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao excluir ingrediente: {ex.Message}";
                return View("ExcluirExibirView", o_IngredienteVM);
            }
        }
    }
}