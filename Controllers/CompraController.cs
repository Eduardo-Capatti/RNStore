using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class CompraController: Controller
{
    private ICompraRepository repository;

    public CompraController (ICompraRepository repository)
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
    public ActionResult Update(int idCompra)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        Compra result = new Compra();

        result.idCompra = idCompra;

        result.compras = repository.Read(idCompra);

        return View(result);
    }

    [HttpPost]
    public ActionResult Update(Compra compra)
    {   
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }
        
        if (compra.statusCompra == 0)
        {
            Compra result = new Compra();

            result.idCompra = compra.idCompra;

            result.compras = repository.Read(compra.idCompra);

            ViewBag.Error = "Preencha todas as informações!";
            return View("Update", result);
        }

        repository.Update(compra);
        return RedirectToAction("Index");
    }

}
