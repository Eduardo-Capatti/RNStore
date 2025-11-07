/*using Microsoft.AspNetCore.Mvc;
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

            repository.Create(cliente); 

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
}*/