using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class CalcadoController: Controller
{
    private ICalcadoRepository repository;

    public CalcadoController (ICalcadoRepository repository)
    {
        this.repository = repository;
    }
    public ActionResult Index()
    {
        return View(repository.Read());
    }
    
    [HttpGet]
    public ActionResult Create()
    {
        var model = repository.Create();
        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Calcado calcado)
    {

        if (calcado.nomeCalcado == null)
        {
            ViewBag.Error = "Preencha todas as informações!";
            var model = repository.Create();
            return View("Create", model);
        }

        repository.Create(calcado);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idCalcado)
    {
        var calcado = repository.Read(idCalcado);
        return View(calcado);
    }

    [HttpPost]
    public ActionResult Update(Calcado calcado)
    {
        repository.Update(calcado);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idCalcado)
    {
        try
        {
            repository.Delete(idCalcado);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o funcionário, contate o suporte para mais detalhes!";

            return View("Index", repository.Read());
        }
       
    }


}
