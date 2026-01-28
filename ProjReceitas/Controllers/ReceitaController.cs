using Microsoft.AspNetCore.Mvc;
using ProjReceitas.BancoDados;
using ProjReceitas.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Data; 
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjReceitas.Controllers
{
    [Authorize]
    public class ReceitaController : Controller
    {
        public IActionResult SobreExibir()
        {
            return View("SobreExibirView");
        }

        [AllowAnonymous]
        public IActionResult Selecionar(string tipoFiltro)
        {
            try
            {
                Receita o_ReceitaDAL = new Receita();

                ViewBag.ReceitasDestaque = o_ReceitaDAL.SelecionarReceitasDestaque(5);

                DataTable dtReceitasFiltradas = o_ReceitaDAL.SelecionarFiltro(tipoFiltro);

                var tiposDeReceitaParaFiltro = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Todos", Text = "Todos os Tipos" },
                    new SelectListItem { Value = "Entrada", Text = "Entrada" },
                    new SelectListItem { Value = "Prato Principal", Text = "Prato Principal" },
                    new SelectListItem { Value = "Sobremesa", Text = "Sobremesa" }
                };
                ViewBag.OpcoesDeFiltro = new SelectList(tiposDeReceitaParaFiltro, "Value", "Text", tipoFiltro ?? "Todos");
                ViewBag.FiltroAtual = tipoFiltro;

                return View("SelecionarView", dtReceitasFiltradas);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar página de receitas: {ex.Message}";
                ViewBag.ReceitasDestaque = new DataTable();
                ViewBag.OpcoesDeFiltro = new SelectList(
                    new List<SelectListItem> { new SelectListItem { Value = "Todos", Text = "Todos os Tipos" } },
                    "Value", "Text");
                ViewBag.FiltroAtual = "Todos";
                return View("SelecionarView", new DataTable()); 
            }
        }
        [AllowAnonymous]
        public IActionResult GetImagemReceita(int id)
        {
            try
            {
                Receita o_ReceitaDAL = new Receita();
                o_ReceitaDAL.id_Receita = id;
                DataTable dtReceita = o_ReceitaDAL.SelecionarPorID();

                if (dtReceita != null && dtReceita.Rows.Count > 0)
                {
                    DataRow row = dtReceita.Rows[0];
                    if (row.Table.Columns.Contains("fotoReceita") && row["fotoReceita"] != DBNull.Value &&
                        row.Table.Columns.Contains("fotoReceitaTipoMIME") && row["fotoReceitaTipoMIME"] != DBNull.Value)
                    {
                        byte[] fotoBytes = (byte[])row["fotoReceita"]; 
                        string tipoMime = row["fotoReceitaTipoMIME"].ToString();
                        return File(fotoBytes, tipoMime);
                    }
                }
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        public IActionResult InserirExibir()
        {
            return View("InserirExibirView", new ReceitaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InserirProcessar(ReceitaViewModel o_ReceitaVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Receita o_ReceitaDAL = new Receita();
                    o_ReceitaDAL.nome = o_ReceitaVM.Nome;
                    o_ReceitaDAL.descricao = o_ReceitaVM.Descricao;
                    o_ReceitaDAL.tempPreparo = o_ReceitaVM.Temp_Preparo;
                    o_ReceitaDAL.tipo = o_ReceitaVM.Tipo;
                    o_ReceitaDAL.preco = o_ReceitaVM.Preco;
                    o_ReceitaDAL.ModoPreparo = o_ReceitaVM.ModoPreparo;


                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        o_ReceitaDAL.idUsuario = userId;
                    }
                    else
                    {
                        TempData["MsgErro"] = "Erro ao identificar usuário. Tente fazer login novamente.";
                        return View("InserirExibirView", o_ReceitaVM);
                    }

                    if (o_ReceitaVM.ImagemArquivo != null && o_ReceitaVM.ImagemArquivo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await o_ReceitaVM.ImagemArquivo.CopyToAsync(memoryStream);
                            o_ReceitaDAL.fotoReceita = memoryStream.ToArray();
                            o_ReceitaDAL.fotoReceitaTipoMIME = o_ReceitaVM.ImagemArquivo.ContentType;
                        }
                    }

                    int novoIdReceita = o_ReceitaDAL.Inserir();

                    if (novoIdReceita > 0)
                    {
                        TempData["MsgSucesso"] = "Dados básicos da receita salvos! Agora adicione os ingredientes.";
                        return RedirectToAction("GerenciarIngredientes", new { idReceita = novoIdReceita });
                    }
                    else
                    {
                        TempData["MsgErro"] = "Erro ao salvar a receita. Não foi possível obter o ID.";
                    }
                }
              
                return View("InserirExibirView", o_ReceitaVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao inserir receita: {ex.Message}";
                return View("InserirExibirView", o_ReceitaVM);
            }
        }

        public IActionResult GerenciarIngredientes(int idReceita)
        {
            if (idReceita <= 0)
            {
                TempData["MsgErro"] = "ID da receita inválido.";
                return RedirectToAction("Selecionar");
            }

            Receita receitaDAL = new Receita();
            receitaDAL.id_Receita = idReceita;
            DataTable dtReceita = receitaDAL.SelecionarPorID();

            if (dtReceita == null || dtReceita.Rows.Count == 0)
            {
                TempData["MsgErro"] = "Receita não encontrada ou você não tem permissão para acessá-la.";
                return RedirectToAction("Selecionar");
            }

            DataRow rowReceita = dtReceita.Rows[0];

            ReceitaIngrediente riDAL = new ReceitaIngrediente();
            riDAL.id_Receita = idReceita;
            DataTable dtIngredientesDaReceita = riDAL.SelecionarPorReceita();
            List<IngredienteNaReceitaItemViewModel> listaDeIngredientesAtuais = new List<IngredienteNaReceitaItemViewModel>();

            if (dtIngredientesDaReceita != null)
            {
                foreach (DataRow row in dtIngredientesDaReceita.Rows)
                {
                    listaDeIngredientesAtuais.Add(new IngredienteNaReceitaItemViewModel
                    {
                        IdReceitaIngrediente = row.Field<int>("Id_Receita_Ingrediente"), 
                        IdIngrediente = row.Field<int>("Id_ingrediente"), 
                        NomeIngrediente = row.Field<string>("NomeIngrediente") ?? "Ingrediente Desconhecido",
                        Quantidade = row.Field<string>("Quantidade") ?? "N/A"
                    });
                }
            }

            var viewModel = new GerenciarIngredientesViewModel
            {
                IdReceita = idReceita,
                NomeReceita = rowReceita.Field<string>("Nome") ?? "Receita sem nome",
                IngredientesAtuais = listaDeIngredientesAtuais
            };

            return View("GerenciarIngredientesView", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarIngredienteAReceita(GerenciarIngredientesViewModel viewModel)
        {
            if (viewModel.IdReceita <= 0)
            {
                TempData["MsgErro"] = "ID da receita não fornecido.";
                return RedirectToAction("Selecionar");
            }

            if (string.IsNullOrWhiteSpace(viewModel.NomeNovoIngrediente) || string.IsNullOrWhiteSpace(viewModel.QuantidadeNovoIngrediente))
            {
                TempData["MsgErro"] = "Nome do ingrediente e quantidade são obrigatórios.";
                return RedirectToAction("GerenciarIngredientes", new { idReceita = viewModel.IdReceita });
            }

            try
            {
                Ingrediente ingredienteDAL = new Ingrediente { nome = viewModel.NomeNovoIngrediente };
                DataTable dtIngrediente = ingredienteDAL.SelecionarPorNome();
                int idIngredienteParaAdicionar;

                if (dtIngrediente != null && dtIngrediente.Rows.Count > 0)
                {
                    idIngredienteParaAdicionar = dtIngrediente.Rows[0].Field<int>("Id_ingrediente");
                }
                else
                {
                    idIngredienteParaAdicionar = ingredienteDAL.Inserir();
                    if (idIngredienteParaAdicionar <= 0)
                    {
                        TempData["MsgErro"] = "Falha ao criar o novo ingrediente.";
                        return RedirectToAction("GerenciarIngredientes", new { idReceita = viewModel.IdReceita });
                    }
                }

                ReceitaIngrediente riVerificaDAL = new ReceitaIngrediente();
                if (riVerificaDAL.Existe(viewModel.IdReceita, idIngredienteParaAdicionar))
                {
                    TempData["MsgErro"] = $"O ingrediente '{viewModel.NomeNovoIngrediente}' já existe nesta receita.";
                }
                else
                {
                    ReceitaIngrediente riDAL = new ReceitaIngrediente
                    {
                        id_Receita = viewModel.IdReceita,
                        id_ingrediente = idIngredienteParaAdicionar,
                        quantidade = viewModel.QuantidadeNovoIngrediente
                    };

                    if (riDAL.Inserir())
                    {
                        TempData["MsgSucesso"] = $"Ingrediente '{viewModel.NomeNovoIngrediente}' adicionado.";
                    }
                    else
                    {
                        TempData["MsgErro"] = $"Falha ao adicionar o ingrediente '{viewModel.NomeNovoIngrediente}'.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao adicionar ingrediente: {ex.Message}";
            }
            return RedirectToAction("GerenciarIngredientes", new { idReceita = viewModel.IdReceita });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverIngredienteDaReceita(int idReceitaIngrediente, int idReceita)
        {
            if (idReceitaIngrediente <= 0 || idReceita <= 0)
            {
                TempData["MsgErro"] = "Informações inválidas para remover o ingrediente.";
                return RedirectToAction("GerenciarIngredientes", new { idReceita = idReceita }); 
            }

            try
            {
                ReceitaIngrediente riDAL = new ReceitaIngrediente { id_Receita_Ingrediente = idReceitaIngrediente };
                if (riDAL.Excluir())
                {
                    TempData["MsgSucesso"] = "Ingrediente removido da receita.";
                }
                else
                {
                    TempData["MsgErro"] = "Falha ao remover o ingrediente da receita.";
                }
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao remover ingrediente: {ex.Message}";
            }
            return RedirectToAction("GerenciarIngredientes", new { idReceita = idReceita });
        }

        [AllowAnonymous]
        public IActionResult Detalhes(int idReceita)
        {
            if (idReceita <= 0)
            {
                TempData["MsgErro"] = "Receita não especificada.";
                return RedirectToAction("Selecionar");
            }

            try
            {
                var viewModel = CarregarDetalhesReceita(idReceita);
                if (viewModel == null)
                {
                    return RedirectToAction("Selecionar");
                }
                return View("DetalhesView", viewModel);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao carregar detalhes da receita: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Erro em Detalhes: {ex.ToString()}"); 
                return RedirectToAction("Selecionar");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarComentario(NovoComentarioViewModel novoComentarioVM) 
        {
            if (novoComentarioVM.IdReceita <= 0)
            {
                TempData["MsgErro"] = "Receita não especificada para adicionar o comentário (ID não recebido).";
                return RedirectToAction("Selecionar");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioIdAutenticado))
            {
                TempData["MsgErro"] = "Você precisa estar logado para comentar.";
                return RedirectToAction("Login", "Conta", new { returnUrl = Url.Action("Detalhes", "Receita", new { idReceita = novoComentarioVM.IdReceita }) });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Comentario comentarioDAL = new Comentario
                    {
                        id_Receita = novoComentarioVM.IdReceita,
                        id_Usuario = usuarioIdAutenticado,
                        texto_Comentario = novoComentarioVM.TextoComentario,
                        nota = novoComentarioVM.Nota
                    };
                    comentarioDAL.Inserir();
                    TempData["MsgSucesso"] = "Comentário adicionado com sucesso!";
                    return RedirectToAction("Detalhes", new { idReceita = novoComentarioVM.IdReceita });
                }
                catch (Exception ex)
                {
                    TempData["MsgErro"] = $"Erro interno ao salvar comentário: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine($"Erro em AdicionarComentario (save): {ex.ToString()}");
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["MsgErroParaComentario"] = "Falha ao adicionar comentário: " + string.Join("; ", errors);
            }

            var viewModelParaRetorno = CarregarDetalhesReceita(novoComentarioVM.IdReceita);
            if (viewModelParaRetorno == null)
            {
                return RedirectToAction("Selecionar");
            }
            viewModelParaRetorno.NovoComentario = novoComentarioVM;
            return View("DetalhesView", viewModelParaRetorno);
        }

        private DetalhesReceitaViewModel CarregarDetalhesReceita(int idReceita)
        {
            Receita receitaDAL = new Receita { id_Receita = idReceita };
            DataTable dtReceita = receitaDAL.SelecionarPorID();

            if (dtReceita == null || dtReceita.Rows.Count == 0)
            {
                TempData["MsgErro"] = "Receita não encontrada.";
                return null;
            }
            DataRow rowReceita = dtReceita.Rows[0];

            ReceitaIngrediente riDAL = new ReceitaIngrediente { id_Receita = idReceita };
            DataTable dtIngredientes = riDAL.SelecionarPorReceita();
            List<IngredienteNaReceitaItemViewModel> ingredientesVM = new List<IngredienteNaReceitaItemViewModel>();
            if (dtIngredientes != null)
            {
                foreach (DataRow rowIng in dtIngredientes.Rows)
                {
                    ingredientesVM.Add(new IngredienteNaReceitaItemViewModel
                    {
                        IdReceitaIngrediente = rowIng.Table.Columns.Contains("Id_Receita_Ingrediente") ? rowIng.Field<int?>("Id_Receita_Ingrediente") ?? 0 : 0,
                        IdIngrediente = rowIng.Field<int>("Id_Ingrediente"),
                        NomeIngrediente = rowIng.Field<string>("NomeIngrediente") ?? "Ingrediente desconhecido",
                        Quantidade = rowIng.Field<string>("Quantidade") ?? "N/A"
                    });
                }
            }

            Comentario comentarioDALBusca = new Comentario();
            DataTable dtComentarios = comentarioDALBusca.SelecionarPorReceita(idReceita);
            List<ComentarioViewModel> comentariosVM = new List<ComentarioViewModel>();
            double somaNotas = 0;
            if (dtComentarios != null)
            {
                foreach (DataRow rowCom in dtComentarios.Rows)
                {
                    comentariosVM.Add(new ComentarioViewModel
                    {
                        IdComentario = rowCom.Field<int>("id_Comentario"),
                        NomeUsuario = rowCom.Field<string>("NomeUsuario") ?? "Usuário Anônimo",
                        TextoComentario = rowCom.Field<string>("texto_Comentario") ?? "",
                        Nota = rowCom.Field<int>("nota"),
                        DataPostagem = DateTime.Now 
                    });
                    if (rowCom["nota"] != DBNull.Value) 
                    {
                        somaNotas += rowCom.Field<int>("nota");
                    }
                }
            }

            string nomeAutor = "Desconhecido";
            if (rowReceita.Table.Columns.Contains("Id_Usuario") && rowReceita["Id_Usuario"] != DBNull.Value)
            {
                Usuario autorDAL = new Usuario { idUsuario = rowReceita.Field<int>("Id_Usuario") };
                DataTable dtAutor = autorDAL.SelecionarPorID();
                if (dtAutor != null && dtAutor.Rows.Count > 0)
                {
                    nomeAutor = dtAutor.Rows[0].Field<string>("Nome") ?? "Autor Desconhecido";
                }
            }

            
            var viewModel = new DetalhesReceitaViewModel
            {
                IdReceita = idReceita,
                NomeReceita = rowReceita.Field<string>("Nome") ?? "Receita sem nome",
                Descricao = rowReceita.Field<string>("Descricao") ?? "Sem descrição",
                TempoPreparo = rowReceita.Field<int?>("Temp_Preparo"),
                Tipo = rowReceita.Field<string>("Tipo") ?? "Não especificado",
                PrecoMedio = rowReceita.Field<decimal?>("preco"), 
                ModoPreparo = rowReceita.Table.Columns.Contains("Modo_Preparo") ? rowReceita.Field<string>("Modo_Preparo") ?? "Não informado." : "Não informado.",
                AutorReceita = nomeAutor,
                IngredientesDaReceita = ingredientesVM,
                Comentarios = comentariosVM,
                TotalAvaliacoes = comentariosVM.Count,
                MediaAvaliacoes = comentariosVM.Count > 0 && somaNotas > 0 ? Math.Round(somaNotas / comentariosVM.Count, 1) : 0,
                NovoComentario = new NovoComentarioViewModel { IdReceita = idReceita }
            };
            return viewModel;
        }
        private bool UsuarioPodeModificarReceita(int idReceita, out Receita receitaDALInstance)
        {
            receitaDALInstance = null; 

            Receita receitaObjParaVerificacao = new Receita { id_Receita = idReceita };
            DataTable dtReceita = receitaObjParaVerificacao.SelecionarPorID();

            if (dtReceita == null || dtReceita.Rows.Count == 0)
            {
                TempData["MsgErro"] = "Receita não encontrada.";
                return false;
            }

            DataRow rowReceita = dtReceita.Rows[0];

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioLogadoId))
            {
                TempData["MsgAviso"] = "Você precisa estar logado para realizar esta ação.";
                return false;
            }

            int? idAutorReceita = rowReceita.Field<int?>("Id_Usuario");

            if (!idAutorReceita.HasValue || idAutorReceita.Value != usuarioLogadoId)
            {

                TempData["MsgErro"] = "Você não tem permissão para modificar esta receita.";
                return false;
            }

            receitaDALInstance = new Receita 
            {
                id_Receita = rowReceita.Field<int>("Id_Receita"),
                nome = rowReceita.Field<string>("Nome"),
                descricao = rowReceita.Field<string>("Descricao"),
                tempPreparo = rowReceita.Field<int?>("Temp_Preparo"),
                tipo = rowReceita.Field<string>("Tipo"),
                preco = rowReceita.Field<decimal?>("Preco"),
                ModoPreparo = rowReceita.Table.Columns.Contains("Modo_Preparo") ? rowReceita.Field<string>("Modo_Preparo") : null,
                idUsuario = idAutorReceita.Value 
            };

            return true; 
        }


        [HttpGet]
        public IActionResult AlterarExibir(int idReceita)
        {
            if (idReceita <= 0)
            {
                TempData["MsgErro"] = "ID da receita inválido.";
                return RedirectToAction("Selecionar");
            }

            if (!UsuarioPodeModificarReceita(idReceita, out Receita receitaDAL))
            {
                return RedirectToAction("Detalhes", new { idReceita });
            }

            var viewModel = new ReceitaViewModel
            {
                Id_Receita = receitaDAL.id_Receita.Value, 
                Nome = receitaDAL.nome,
                Descricao = receitaDAL.descricao,
                Temp_Preparo = receitaDAL.tempPreparo,
                Tipo = receitaDAL.tipo,
                Preco = receitaDAL.preco,
                ModoPreparo = receitaDAL.ModoPreparo
            
            };
            return View("AlterarExibirView", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarProcessar(ReceitaViewModel viewModel)
        {
            if (viewModel.Id_Receita <= 0)
            {
                TempData["MsgErro"] = "ID da receita inválido para alteração.";
                ModelState.AddModelError("IdReceita", "ID da receita é obrigatório para alteração.");
                return View("AlterarExibirView", viewModel);
            }

            if (ModelState.ContainsKey("ImagemArquivo"))
            {
                ModelState.Remove("ImagemArquivo");
            }


            if (ModelState.IsValid)
            {
                if (!UsuarioPodeModificarReceita(viewModel.Id_Receita, out Receita receitaOriginalDAL))
                {
                    TempData["MsgErro"] = TempData["MsgErro"]?.ToString() ?? "Você não tem permissão ou a receita não foi encontrada para alteração.";
                    return View("AlterarExibirView", viewModel);
                }

                Receita receitaParaAtualizar = new Receita
                {
                    id_Receita = viewModel.Id_Receita,
                    nome = viewModel.Nome,
                    descricao = viewModel.Descricao,
                    tempPreparo = viewModel.Temp_Preparo,
                    tipo = viewModel.Tipo,
                    preco = viewModel.Preco,
                    ModoPreparo = viewModel.ModoPreparo,
                    idUsuario = receitaOriginalDAL.idUsuario 
                };

                if (viewModel.ImagemArquivo != null && viewModel.ImagemArquivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.ImagemArquivo.CopyToAsync(memoryStream);
                        receitaParaAtualizar.fotoReceita = memoryStream.ToArray();
                        receitaParaAtualizar.fotoReceitaTipoMIME = viewModel.ImagemArquivo.ContentType;
                    }
                }

                try
                {
                    receitaParaAtualizar.Update();
                    TempData["MsgSucesso"] = "Receita atualizada com sucesso!";
                    return RedirectToAction("Detalhes", new { idReceita = viewModel.Id_Receita });
                }
                catch (Exception ex)
                {
                    TempData["MsgErro"] = $"Erro ao atualizar receita: {ex.Message}";
                    // Log ex para debugging
                    System.Diagnostics.Debug.WriteLine($"Erro ao atualizar receita {viewModel.Id_Receita}: {ex}");
                }
            }

            TempData["MsgErro"] = TempData["MsgErro"]?.ToString() ?? "Verifique os erros no formulário."; 
            return View("AlterarExibirView", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirProcessar(int idReceita)
        {
            if (idReceita <= 0)
            {
                TempData["MsgErro"] = "ID da receita inválido para exclusão.";
                return RedirectToAction("Selecionar"); 
            }

       
            if (!UsuarioPodeModificarReceita(idReceita, out Receita _)) 
            {
              
                return RedirectToAction("Detalhes", new { idReceita = idReceita });
            }

            try
            {
                Receita receitaParaExcluir = new Receita { id_Receita = idReceita };
                receitaParaExcluir.Excluir();

                TempData["MsgSucesso"] = "Receita excluída com sucesso!";
                return RedirectToAction("Selecionar");
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro ao excluir receita: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Erro ao excluir receita ID {idReceita}: {ex.ToString()}"); 
                return RedirectToAction("Detalhes", new { idReceita = idReceita });
            }
        }

    }
}

    