using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class MarcaController: Controller
{
    private IMarcaRepository repository;

    public MarcaController (IMarcaRepository repository)
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
    public ActionResult Create(Marca marca)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        if (marca.nomeMarca == null)
        {
            var marcas = repository.Read();
            ViewBag.Error = "Preencha o nome da marca!";
            return View("Index", marcas);
        }
        else
        {
            repository.Create(marca);

            return RedirectToAction("Index");
        }
        
    }


    public ActionResult Delete(int idMarca)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idMarca);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir a marca, pois ela está sendo usada em algum produto!";

            return View("Index", repository.Read());
        }
     
    }


}
