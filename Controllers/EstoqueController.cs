using Microsoft.AspNetCore.Mvc;

namespace RNStore.Controllers;

public class EstoqueController: Controller
{
    public ActionResult Index()
    {
        return View();
    }
}
