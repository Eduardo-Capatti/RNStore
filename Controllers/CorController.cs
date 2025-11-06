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
    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpPost]
    public ActionResult Create(Cor Cor)
    {
        repository.Create(Cor);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idCor)
    {
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
