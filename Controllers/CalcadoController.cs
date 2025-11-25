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
    
    [HttpGet]
    public ActionResult Create()
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        var model = repository.Create();
        return View(model);
    }

    [HttpPost]
    public ActionResult Create(Calcado calcado)
    {

        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        if (calcado.nomeCalcado == null || calcado.marcaId == 0)
        {
            ViewBag.Error = "Preencha todas as informações!";
            ViewBag.Calcado = calcado;
            var model = repository.Create();
            return View("Create", model);
        }

        repository.Create(calcado);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idCalcado)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        var calcado = repository.Read(idCalcado);
        return View(calcado);
    }

    [HttpPost]
    public ActionResult Update(Calcado calcado)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Calcado calcado)
        {
            if (calcado == null) return true;

            return calcado.GetType()
                .GetProperties()
                .Where(p => p.CanRead && (p.Name == "nomeCalcado" || p.Name == "marcaId"))
                .Any(p => p.GetValue(calcado) == null);
        }

        if (TemCampoNulo(calcado))
        {
            ViewBag.Error = "Não deixe informações em branco!";
            var calcado_escolhido = repository.Read(calcado.idCalcado);
            return View("Update", calcado_escolhido);
        }

        repository.Update(calcado);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idCalcado)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idCalcado);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o calçado, pois ele está sendo usado!";

            return View("Index", repository.Read());
        }
       
    }


}
