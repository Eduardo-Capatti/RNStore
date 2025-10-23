using Microsoft.AspNetCore.Mvc;
using RNStore.Models;
//using RNStore.Repositories;

namespace Produto.Controllers;

public class ProdutoController: Controller
{
    public ActionResult Index()
    {
        return View();
    }
    public IActionResult Inicio()
    {
        return View("~/Views/Produto/Inicio.cshtml");
    }
}
