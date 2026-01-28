using Microsoft.AspNetCore.Mvc;
using ProjReceitas.BancoDados;
using ProjReceitas.Models;
using ProjReceitas.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ProjReceitas.Controllers
{
    public class ContaController : Controller
    {
        [AllowAnonymous]
        public IActionResult Cadastro()
        {
            CadastroViewModel o_CadastroVM = new CadastroViewModel();
            return View("CadastroView", o_CadastroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult CadastroProcessar(CadastroViewModel o_CadastroVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Usuario o_UsuarioVerifica = new Usuario();
                    if (o_UsuarioVerifica.VerificarSeUsuarioExiste(o_CadastroVM.NomeUsuario))
                    {
                        ModelState.AddModelError("NomeUsuario", "Este nome de usuário já está em uso.");
                        return View("CadastroView", o_CadastroVM);
                    }

                    Usuario o_UsuarioNovo = new Usuario();
                    int novoUsuarioId = o_UsuarioNovo.Inserir(o_CadastroVM.NomeUsuario, o_CadastroVM.Senha);

                    if (novoUsuarioId > 0)
                    {
                        TempData["MsgSucesso"] = "Cadastro realizado com sucesso! Você já pode fazer o login.";
                        return RedirectToAction("InserirExibir", "Receita");
                    }
                    else
                    {
                        TempData["MsgErro"] = "Ocorreu um erro ao tentar realizar o cadastro. Não foi possível obter o ID.";
                        return View("CadastroView", o_CadastroVM);
                    }
                }
                return View("CadastroView", o_CadastroVM);
            }
            catch (Exception ex)
            {
                TempData["MsgErro"] = $"Erro inesperado durante o cadastro: {ex.Message}";
                return View("CadastroView", o_CadastroVM);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            LoginViewModel o_LoginVM = new LoginViewModel();
            return View("LoginView", o_LoginVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginProcessar(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                Usuario usuarioDAL = new Usuario();
                usuarioDAL.Nome = model.NomeUsuario;

                DataTable dtUsuario = usuarioDAL.SelecionarPorNome();

                if (dtUsuario != null && dtUsuario.Rows.Count > 0)
                {
                    DataRow userDataRow = dtUsuario.Rows[0];
                    string senhaHashArmazenada = userDataRow["SenhaHash"].ToString();
                    int usuarioId = Convert.ToInt32(userDataRow["Id_Usuario"]);
                    string nomeUsuarioDoBanco = userDataRow["Nome"].ToString();

                    if (PasswordHasher.VerifyPassword(model.Senha, senhaHashArmazenada))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                            new Claim(ClaimTypes.Name, nomeUsuarioDoBanco)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        TempData["MsgSucesso"] = $"Bem-vindo de volta, {nomeUsuarioDoBanco}!";

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return RedirectToAction("Selecionar", "Receita");
                        }
                        return RedirectToAction("Selecionar", "Receita");
                    }
                    else
                    {
                        TempData["MsgErro"] = "Senha inválida. Por favor, tente novamente.";
                        return View("LoginView", model);
                    }
                }
                else
                {
                    TempData["MsgErro"] = "Nome de usuário não encontrado.";
                    return View("LoginView", model);
                }
            }

            return View("LoginView", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutProcessar()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["MsgSucesso"] = "Você saiu do sistema com sucesso.";
            return RedirectToAction("Selecionar", "Receita");
        }

        [AllowAnonymous]
        public IActionResult AcessoNegado()
        {
            return View("AcessoNegadoView");
        }
    }
}