using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class TamanhoController: Controller
{
    private ITamanhoRepository repository;

    public TamanhoController (ITamanhoRepository repository)
    {
        this.repository = repository;
    }

    public bool VerificaSession()
    {
        int? idUsuario = HttpContext.Session.GetInt32("idUsuario");

        if (idUsuario == null)
        {
            return true;
        }

        return false;
    }

    public ActionResult Index()
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        return View(repository.Read());
    }

    [HttpPost]
    public ActionResult Create(Tamanho tamanho)
    {   
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }
        if (tamanho.tamanho == null)
        {
            var tamanhos = repository.Read();
            ViewBag.Error = "Preencha o tamanho!";
            return View("Index", tamanhos);
        }
        repository.Create(tamanho);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idTamanho)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idTamanho);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o tamanho, pois ele está sendo usado em algum produto!";

            return View("Index", repository.Read());
        }
     
    }


}
