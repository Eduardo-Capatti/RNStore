using Microsoft.AspNetCore.Mvc;
using RNStore.Models;
using RNStore.Repositories;
using Microsoft.Data.SqlClient; 

namespace RNStore.Controllers;

public class ClienteController : Controller
{
    private IClienteRepository repository;

    public ClienteController(IClienteRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    public ActionResult Cadastro()
    {
        return View(); 
    }

    [HttpPost]
    public ActionResult Create(Cliente cliente)
    {
        try
        {
            string confirmarSenha = Request.Form["ConfirmarSenha"];
            if (cliente.Senha != confirmarSenha)
            {
                ViewBag.Error = "As senhas não conferem. Tente novamente.";
                return View("Cadastro", cliente); 
            }
            
            bool TemCampoNulo(Cliente cli)
            {
                if (cli == null) return true;

                return cli.GetType()
                    .GetProperties()
                    .Where(p => p.CanRead &&
                                p.Name != "IdPessoa" &&
                                p.Name != "Enderecos" &&
                                p.Name != "Telefone") 
                    .Any(p => p.GetValue(cli) == null);
            }

            if (TemCampoNulo(cliente))
            {
                ViewBag.Error = "Preencha todas as informações obrigatórias!";
                return View("Cadastro", cliente); 
            }

            var infos = repository.Create(cliente); 

            HttpContext.Session.SetInt32("idCliente", infos.idUsuario);
            HttpContext.Session.SetString("nomeCliente", infos.nomeUsuario);


            return RedirectToAction("Index", "Catalogo"); 
        }
        catch (SqlException e)
        {
            if (e.Message.Contains("UNIQUE KEY"))
            {
                ViewBag.Error = "Este E-mail ou CPF já está em uso.";
            }
            else
            {
                ViewBag.Error = "Ocorreu um erro no banco de dados ao salvar.";
            }
            return View("Cadastro", cliente);
        }
        catch(Exception e)
        {
            ViewBag.Error = "Ocorreu um erro inesperado: " + e.Message;
            return View("Cadastro", cliente);
        }
    }

    public ActionResult Login()
    {
        return View(new Login());
    }

    [HttpPost]
    public ActionResult Login(Login model)
    {
        User user = repository.LoginUser(model);

        if (user == null)
        {
            ViewBag.Error = "Usuário e/ou senha inválidos!";
            return View(model);
        }

        HttpContext.Session.SetInt32("idCliente", user.idUsuario);
        HttpContext.Session.SetString("nomeCliente", user.nomeUsuario);

        if(HttpContext.Session.GetInt32("idProduto") != null)
        {
            var idCliente = HttpContext.Session.GetInt32("idCliente") ?? 0;
            var idProduto = HttpContext.Session.GetInt32("idProduto") ?? 0;
            repository.ColocarCarrinho(idCliente, idProduto);

            return RedirectToAction("Carrinho", "Catalogo");
        }

        return RedirectToAction("Index", "Catalogo");
    }

    public ActionResult Sair()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Catalogo");
    }

}
