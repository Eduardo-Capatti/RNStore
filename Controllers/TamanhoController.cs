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
    public ActionResult Index()
    {
        return View(repository.Read());
    }

    [HttpPost]
    public ActionResult Create(Tamanho tamanho)
    {   
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
