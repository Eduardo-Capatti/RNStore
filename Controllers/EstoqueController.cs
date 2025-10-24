using Microsoft.AspNetCore.Mvc;

namespace Estoque.Controllers;

public class EstoqueController: Controller
{
    public ActionResult Index()
    {
        return View();
    }

}
