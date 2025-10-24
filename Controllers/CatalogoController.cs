using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Controllers;

public class CatalogoController : Controller
{

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Produto(int id)
    {
        return View();
    }
}
