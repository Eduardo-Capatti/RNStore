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
    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpPost]
    public ActionResult Create(Marca marca)
    {
        repository.Create(marca);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idMarca)
    {
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
