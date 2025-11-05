using Microsoft.AspNetCore.Mvc;
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
        repository.Create(tamanho);

        return RedirectToAction("Index");
    }


    public ActionResult Update(Tamanho tamanho)
    {
        repository.Create(tamanho);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idTamanho)
    {
        repository.Delete(idTamanho);

        return RedirectToAction("Index");
    }


}
