using Microsoft.AspNetCore.Mvc;

namespace RNStore.Controllers;

public class EstoqueController: Controller
{
    public ActionResult Index()
    {
        int? idUsuario = HttpContext.Session.GetInt32("idUsuario");

        if (idUsuario == null)
        {
            return RedirectToAction("Login", "User");
        }

        ViewBag.idUsuario = idUsuario;
        return View();
    }
}
