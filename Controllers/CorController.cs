using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class CorController: Controller
{
    private ICorRepository repository;

    public CorController (ICorRepository repository)
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
    public ActionResult Create(Cor cor)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        if (cor.nomeCor == null)
        {
            var cores = repository.Read();
            ViewBag.Error = "Preencha a cor!";
            return View("Index", cores);
        }
        else
        {
            repository.Create(cor);

            return RedirectToAction("Index");
        }

        
    }


    public ActionResult Delete(int idCor)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idCor);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir a cor, pois ela está sendo usada em algum produto!";

            return View("Index", repository.Read());
        }
     
    }


}
